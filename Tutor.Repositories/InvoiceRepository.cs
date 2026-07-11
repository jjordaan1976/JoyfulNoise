using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IInvoiceDataAccessObject _invoiceService;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IInvoiceDataAccessObject invoiceService, ILogger<InvoiceRepository> logger)
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
            await _invoiceService.InsertBatchAsync(invoices, tx, connection);
            return true;
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            return await _invoiceService.UpdateStatusAsync(invoiceId, status, paidDate);
        }
    }
}
