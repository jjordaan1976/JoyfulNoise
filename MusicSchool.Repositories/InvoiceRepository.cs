using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IInvoiceService invoiceService, ILogger<InvoiceRepository> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            return await _invoiceService.GetInvoiceAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            return await _invoiceService.GetByBundleAsync(bundleId);
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _invoiceService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            return await _invoiceService.GetOutstandingByAccountHolderAsync(accountHolderId);
        }

        /// <summary>
        /// Saves all 12 instalment rows for a bundle atomically.
        /// The application layer is responsible for calculating the Amount
        /// and setting the DueDate for each instalment before calling this method.
        /// </summary>
        public async Task<bool> AddInvoiceInstalmentsAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            try
            {
                await _invoiceService.InsertBatchAsync(invoices, tx, connection);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert invoice instalments for BundleID {BundleID}",
                    invoices.FirstOrDefault()?.BundleID);
                return false;
            }
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            try
            {
                return await _invoiceService.UpdateStatusAsync(invoiceId, status, paidDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for InvoiceID {InvoiceID}", invoiceId);
                return false;
            }
        }
    }
}
