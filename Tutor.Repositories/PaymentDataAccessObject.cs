using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class PaymentDataAccessObject : IPaymentDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE PaymentID = @PaymentID;";

        public static readonly string GetByAccountHolderSql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                ORDER BY PaymentDate DESC, CreatedAt DESC;";

        public static readonly string InsertSql = @"
                INSERT INTO Payment
                    (AccountHolderID, Amount, UnallocatedAmount, PaymentDate,
                     Source, Reference, Notes)
                VALUES
                    (@AccountHolderID, @Amount, @UnallocatedAmount, @PaymentDate,
                     @Source, @Reference, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateUnallocatedSql = @"
                UPDATE Payment
                SET UnallocatedAmount = @UnallocatedAmount
                WHERE PaymentID = @PaymentID;";

        public static readonly string GetAllocationsByPaymentSql = @"
                SELECT AllocationID, PaymentID, InvoiceID, AmountApplied, CreatedAt
                FROM PaymentAllocation
                WHERE PaymentID = @PaymentID;";

        public static readonly string GetAllocationsByInvoiceSql = @"
                SELECT AllocationID, PaymentID, InvoiceID, AmountApplied, CreatedAt
                FROM PaymentAllocation
                WHERE InvoiceID = @InvoiceID;";

        public static readonly string InsertAllocationSql = @"
                INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

        public static readonly string GetTotalUnallocatedSql = @"
                SELECT ISNULL(SUM(UnallocatedAmount), 0)
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                  AND UnallocatedAmount > 0;";

        public PaymentDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Payment?> GetPaymentAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<Payment>(GetByIdSql, new { PaymentID = id });
        }

        public async Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _connection.QueryAsync<Payment>(GetByAccountHolderSql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Payment payment)
        {
            return await _connection.ExecuteScalarAsync<int>(InsertSql, payment);
        }

        public async Task<bool> UpdateUnallocatedAsync(int paymentId, decimal unallocatedAmount)
        {
            var rows = await _connection.ExecuteAsync(UpdateUnallocatedSql,
                new { PaymentID = paymentId, UnallocatedAmount = unallocatedAmount });
            return rows > 0;
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId)
        {
            return await _connection.QueryAsync<PaymentAllocation>(GetAllocationsByPaymentSql, new { PaymentID = paymentId });
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId)
        {
            return await _connection.QueryAsync<PaymentAllocation>(GetAllocationsByInvoiceSql, new { InvoiceID = invoiceId });
        }

        public async Task InsertAllocationAsync(PaymentAllocation allocation)
        {
            await _connection.ExecuteAsync(InsertAllocationSql, allocation);
        }

        public async Task<decimal> GetTotalUnallocatedAsync(int accountHolderId)
        {
            return await _connection.ExecuteScalarAsync<decimal>(GetTotalUnallocatedSql,
                new { AccountHolderID = accountHolderId });
        }
    }
}
