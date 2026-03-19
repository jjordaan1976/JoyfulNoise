using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetInvoiceAsync(int id);
        Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId);

        /// <summary>Inserts multiple invoice rows within an existing transaction (used for bundle instalments).</summary>
        Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection);

        /// <summary>Inserts a single invoice row within an existing transaction (used for extra-lesson invoices).</summary>
        Task<int> InsertAsync(Invoice invoice, IDbTransaction tx, IDbConnection connection);

        Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate);
    }
}
