using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
{
    [Route("Invoice")]
    public class InvoiceController : BaseController
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        [HttpGet("GetInvoice")]
        public Task<ResponseBase<Invoice?>> GetInvoice([FromQuery] int id)
            => Execute(() => _invoiceRepository.GetInvoiceAsync(id), _logger, "Error getting invoice");

        [HttpGet("GetByBundle")]
        public Task<ResponseBase<IEnumerable<Invoice>>> GetByBundle([FromQuery] int bundleId)
            => Execute(() => _invoiceRepository.GetByBundleAsync(bundleId), _logger, "Error getting invoices by bundle");

        [HttpGet("GetByAccountHolder")]
        public Task<ResponseBase<IEnumerable<Invoice>>> GetByAccountHolder([FromQuery] int accountHolderId)
            => Execute(() => _invoiceRepository.GetByAccountHolderAsync(accountHolderId), _logger, "Error getting invoices by account holder");

        [HttpGet("GetOutstandingByAccountHolder")]
        public Task<ResponseBase<IEnumerable<Invoice>>> GetOutstandingByAccountHolder([FromQuery] int accountHolderId)
            => Execute(() => _invoiceRepository.GetOutstandingByAccountHolderAsync(accountHolderId), _logger, "Error getting outstanding invoices by account holder");

        [HttpPut("UpdateInvoiceStatus")]
        public Task<ResponseBase<bool>> UpdateInvoiceStatus([FromQuery] int invoiceId, [FromQuery] string status, [FromQuery] DateOnly? paidDate)
            => Execute(() => _invoiceRepository.UpdateInvoiceStatusAsync(invoiceId, status, paidDate), _logger, "Error updating invoice status");
    }
}
