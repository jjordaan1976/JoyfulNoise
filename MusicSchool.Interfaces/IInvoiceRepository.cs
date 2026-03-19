using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetInvoiceAsync(int id);
        Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId);
        Task<bool> AddInvoiceInstalmentsAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection);
        Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate);
    }
}
