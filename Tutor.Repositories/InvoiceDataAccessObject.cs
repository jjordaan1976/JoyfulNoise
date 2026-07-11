using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class InvoiceDataAccessObject : IInvoiceDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
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

        public static readonly string GetByBundleSql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
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

        public static readonly string GetByAccountHolderSql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
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

        public static readonly string GetOutstandingByAccountHolderSql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
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

        public static readonly string InsertBatchSql = @"
                INSERT INTO Invoice
                    (BundleID, ExtraLessonID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @ExtraLessonID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);";

        public static readonly string InsertSql = @"
                INSERT INTO Invoice
                    (BundleID, ExtraLessonID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @ExtraLessonID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateStatusSql = @"
                UPDATE Invoice
                SET Status   = @Status,
                    PaidDate = @PaidDate
                WHERE InvoiceID = @InvoiceID;";

        public InvoiceDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<Invoice>(GetByIdSql, new { InvoiceID = id });
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            return await _connection.QueryAsync<Invoice>(GetByBundleSql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _connection.QueryAsync<Invoice>(GetByAccountHolderSql, new { AccountHolderID = accountHolderId });
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            return await _connection.QueryAsync<Invoice>(GetOutstandingByAccountHolderSql, new { AccountHolderID = accountHolderId });
        }

        public async Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(InsertBatchSql, invoices, tx));
        }

        /// <summary>
        /// Inserts a single Invoice row within an existing transaction.
        /// Returns the new InvoiceID.
        /// </summary>
        public async Task<int> InsertAsync(Invoice invoice, IDbTransaction tx, IDbConnection connection)
        {
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(InsertSql, invoice, tx));
        }

        public async Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            DateTime? paidDateTime = paidDate.HasValue
                ? paidDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;

            var rowsAffected = await _connection.ExecuteAsync(UpdateStatusSql,
                new { InvoiceID = invoiceId, Status = status, PaidDate = paidDateTime });
            return rowsAffected > 0;
        }
    }
}
