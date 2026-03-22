using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class PaymentDataAccessObject : IPaymentDataAccessObject
    {
        private readonly IDbConnection _connection;

        public PaymentDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Payment?> GetPaymentAsync(int id)
        {
            const string sql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE PaymentID = @PaymentID;";

            return await _connection.QuerySingleOrDefaultAsync<Payment>(sql, new { PaymentID = id });
        }

        public async Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                ORDER BY PaymentDate DESC, CreatedAt DESC;";

            return await _connection.QueryAsync<Payment>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Payment payment)
        {
            const string sql = @"
                INSERT INTO Payment
                    (AccountHolderID, Amount, UnallocatedAmount, PaymentDate,
                     Source, Reference, Notes)
                VALUES
                    (@AccountHolderID, @Amount, @UnallocatedAmount, @PaymentDate,
                     @Source, @Reference, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, payment);
        }

        public async Task<bool> UpdateUnallocatedAsync(int paymentId, decimal unallocatedAmount)
        {
            const string sql = @"
                UPDATE Payment
                SET UnallocatedAmount = @UnallocatedAmount
                WHERE PaymentID = @PaymentID;";

            var rows = await _connection.ExecuteAsync(sql,
                new { PaymentID = paymentId, UnallocatedAmount = unallocatedAmount });
            return rows > 0;
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId)
        {
            const string sql = @"
                SELECT AllocationID, PaymentID, InvoiceID, AmountApplied, CreatedAt
                FROM PaymentAllocation
                WHERE PaymentID = @PaymentID;";

            return await _connection.QueryAsync<PaymentAllocation>(sql, new { PaymentID = paymentId });
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId)
        {
            const string sql = @"
                SELECT AllocationID, PaymentID, InvoiceID, AmountApplied, CreatedAt
                FROM PaymentAllocation
                WHERE InvoiceID = @InvoiceID;";

            return await _connection.QueryAsync<PaymentAllocation>(sql, new { InvoiceID = invoiceId });
        }

        public async Task InsertAllocationAsync(PaymentAllocation allocation)
        {
            const string sql = @"
                INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

            await _connection.ExecuteAsync(sql, allocation);
        }

        public async Task<decimal> GetTotalUnallocatedAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT ISNULL(SUM(UnallocatedAmount), 0)
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                  AND UnallocatedAmount > 0;";

            return await _connection.ExecuteScalarAsync<decimal>(sql,
                new { AccountHolderID = accountHolderId });
        }
    }
}
