using Dapper;
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    /// <summary>
    /// Orchestrates payment recording and the allocation engine.
    ///
    /// Allocation rules:
    ///  1. Gather all outstanding (Pending/Overdue) invoices for the account holder,
    ///     sorted by DueDate ascending (oldest-first).
    ///  2. Pool the new payment with any existing unallocated amounts.
    ///  3. Walk the invoice list:
    ///       - If pool >= invoice.Amount: drain oldest unallocated payments into the
    ///         invoice, mark it Paid, reduce pool.
    ///       - Otherwise: stop.
    ///  4. Persist updated UnallocatedAmount on every touched Payment row.
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPaymentDataAccessObject _paymentDao;
        private readonly IInvoiceDataAccessObject _invoiceDao;
        private readonly IDbConnection            _connection;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(
            IPaymentDataAccessObject   paymentDao,
            IInvoiceDataAccessObject   invoiceDao,
            IDbConnection              connection,
            ILogger<PaymentRepository> logger)
        {
            _paymentDao = paymentDao;
            _invoiceDao = invoiceDao;
            _connection = connection;
            _logger     = logger;
        }

        public Task<Payment?> GetPaymentAsync(int id)
            => _paymentDao.GetPaymentAsync(id);

        public Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId)
            => _paymentDao.GetByAccountHolderAsync(accountHolderId);

        public Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId)
            => _paymentDao.GetAllocationsByPaymentAsync(paymentId);

        public Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId)
            => _paymentDao.GetAllocationsByInvoiceAsync(invoiceId);

        // ── Add payment + run allocation engine ───────────────────────────────

        public async Task<int?> AddPaymentAsync(Payment payment)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using var tx = _connection.BeginTransaction();
                try
                {
                    // Start fully unallocated; engine will reduce it.
                    payment.UnallocatedAmount = payment.Amount;
                    var paymentId = await InsertPaymentInTxAsync(payment, tx);

                    await RunAllocationEngineAsync(payment.AccountHolderID, tx);

                    tx.Commit();
                    return paymentId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to add payment for AccountHolderID {AccountHolderID}",
                    payment.AccountHolderID);
                return null;
            }
        }

        public async Task<int?> QuickPayInvoiceAsync(int invoiceId, DateTime paymentDate)
        {
            try
            {
                var invoice = await _invoiceDao.GetInvoiceAsync(invoiceId);
                if (invoice is null)
                {
                    _logger.LogWarning("QuickPay: InvoiceID {InvoiceID} not found", invoiceId);
                    return null;
                }

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using var tx = _connection.BeginTransaction();
                try
                {
                    var payment = new Payment
                    {
                        AccountHolderID   = invoice.AccountHolderID,
                        Amount            = invoice.Amount,
                        UnallocatedAmount = 0,
                        PaymentDate       = paymentDate,
                        Source            = PaymentSource.QuickPay,
                        Notes             = $"Quick-pay for Invoice #{invoiceId}"
                    };

                    var paymentId = await InsertPaymentInTxAsync(payment, tx);
                    await AllocateToInvoiceAsync(paymentId, invoice, invoice.Amount, tx);

                    tx.Commit();
                    return paymentId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickPay failed for InvoiceID {InvoiceID}", invoiceId);
                return null;
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task<int> InsertPaymentInTxAsync(Payment payment, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO Payment
                    (AccountHolderID, Amount, UnallocatedAmount, PaymentDate,
                     Source, Reference, Notes)
                VALUES
                    (@AccountHolderID, @Amount, @UnallocatedAmount, @PaymentDate,
                     @Source, @Reference, @Notes);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, payment, tx));
        }

        /// <summary>
        /// Loads all unallocated payments and outstanding invoices for the account,
        /// then greedily allocates the pooled funds to invoices (oldest DueDate first).
        /// </summary>
        private async Task RunAllocationEngineAsync(int accountHolderId, IDbTransaction tx)
        {
            // Outstanding invoices, oldest first.
            const string invoiceSql = @"
                SELECT InvoiceID, BundleID, ExtraLessonID, AccountHolderID,
                       InstallmentNumber, Amount, DueDate, PaidDate, Status, Notes, CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                  AND Status IN ('Pending', 'Overdue')
                ORDER BY DueDate ASC, InvoiceID ASC;";

            var invoices = (await _connection.QueryAsync<Invoice>(
                new CommandDefinition(invoiceSql, new { AccountHolderID = accountHolderId }, tx))).ToList();

            if (invoices.Count == 0) return;

            // Payments with remaining unallocated amounts, oldest first.
            const string paymentSql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                  AND UnallocatedAmount > 0
                ORDER BY PaymentDate ASC, PaymentID ASC;";

            var payments = (await _connection.QueryAsync<Payment>(
                new CommandDefinition(paymentSql, new { AccountHolderID = accountHolderId }, tx))).ToList();

            if (payments.Count == 0) return;

            foreach (var invoice in invoices)
            {
                // How much of this invoice is already covered?
                const string coveredSql = @"
                    SELECT ISNULL(SUM(AmountApplied), 0)
                    FROM PaymentAllocation
                    WHERE InvoiceID = @InvoiceID;";

                var alreadyCovered = await _connection.ExecuteScalarAsync<decimal>(
                    new CommandDefinition(coveredSql, new { InvoiceID = invoice.InvoiceID }, tx));

                var needed = invoice.Amount - alreadyCovered;
                if (needed <= 0) continue;

                var pool = payments.Sum(p => p.UnallocatedAmount);
                if (pool < needed) break; // Insufficient funds for the next invoice — stop.

                // Drain payments into this invoice.
                var remaining = needed;
                foreach (var pmt in payments.Where(p => p.UnallocatedAmount > 0))
                {
                    if (remaining <= 0) break;

                    var take = Math.Min(pmt.UnallocatedAmount, remaining);
                    pmt.UnallocatedAmount -= take;
                    remaining -= take;

                    const string allocSql = @"
                        INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                        VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

                    await _connection.ExecuteAsync(new CommandDefinition(allocSql,
                        new { PaymentID = pmt.PaymentID, InvoiceID = invoice.InvoiceID, AmountApplied = take },
                        tx));
                }

                // Mark the invoice Paid.
                const string paidSql = @"
                    UPDATE Invoice SET Status = 'Paid', PaidDate = @PaidDate
                    WHERE InvoiceID = @InvoiceID;";

                await _connection.ExecuteAsync(new CommandDefinition(paidSql,
                    new { InvoiceID = invoice.InvoiceID, PaidDate = DateTime.Today }, tx));
            }

            // Persist updated UnallocatedAmount on all touched payments.
            const string updateSql = @"
                UPDATE Payment SET UnallocatedAmount = @UnallocatedAmount
                WHERE PaymentID = @PaymentID;";

            foreach (var pmt in payments)
            {
                await _connection.ExecuteAsync(new CommandDefinition(updateSql,
                    new { PaymentID = pmt.PaymentID, UnallocatedAmount = pmt.UnallocatedAmount }, tx));
            }
        }

        /// <summary>QuickPay direct allocation (exact-match amount).</summary>
        private async Task AllocateToInvoiceAsync(
            int paymentId, Invoice invoice, decimal amount, IDbTransaction tx)
        {
            const string allocSql = @"
                INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

            await _connection.ExecuteAsync(new CommandDefinition(allocSql,
                new { PaymentID = paymentId, InvoiceID = invoice.InvoiceID, AmountApplied = amount }, tx));

            const string paidSql = @"
                UPDATE Invoice SET Status = 'Paid', PaidDate = @PaidDate
                WHERE InvoiceID = @InvoiceID;";

            await _connection.ExecuteAsync(new CommandDefinition(paidSql,
                new { InvoiceID = invoice.InvoiceID, PaidDate = DateTime.Today }, tx));
        }
    }
}
