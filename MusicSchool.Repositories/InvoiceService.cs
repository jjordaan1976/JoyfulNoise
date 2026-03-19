using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IDbConnection _connection;

        public InvoiceService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE InvoiceID = @InvoiceID;";

            return await _connection.QuerySingleOrDefaultAsync<Invoice>(sql, new { InvoiceID = id });
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE BundleID = @BundleID
                ORDER BY InstallmentNumber;";

            return await _connection.QueryAsync<Invoice>(sql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                ORDER BY DueDate;";

            return await _connection.QueryAsync<Invoice>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                  AND Status IN ('Pending', 'Overdue')
                ORDER BY DueDate;";

            return await _connection.QueryAsync<Invoice>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO Invoice
                    (BundleID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, invoices, tx));
        }

        public async Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            const string sql = @"
                UPDATE Invoice
                SET Status   = @Status,
                    PaidDate = @PaidDate
                WHERE InvoiceID = @InvoiceID;";
            DateTime? paidDateTime = paidDate.HasValue
                ? paidDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;
            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { InvoiceID = invoiceId, Status = status, PaidDate = paidDateTime });
            return rowsAffected > 0;
        }
    }
}
