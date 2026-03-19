using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
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
        public async Task<ResponseBase<Invoice>> GetInvoice([FromQuery] int id)
        {
            ResponseBase<Invoice> response = new ResponseBase<Invoice>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetInvoiceAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByBundle")]
        public async Task<ResponseBase<IEnumerable<Invoice>>> GetByBundle([FromQuery] int bundleId)
        {
            ResponseBase<IEnumerable<Invoice>> response = new ResponseBase<IEnumerable<Invoice>>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetByBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Invoice>>> GetByAccountHolder([FromQuery] int accountHolderId)
        {
            ResponseBase<IEnumerable<Invoice>> response = new ResponseBase<IEnumerable<Invoice>>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetOutstandingByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Invoice>>> GetOutstandingByAccountHolder([FromQuery] int accountHolderId)
        {
            ResponseBase<IEnumerable<Invoice>> response = new ResponseBase<IEnumerable<Invoice>>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetOutstandingByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateInvoiceStatus")]
        public async Task<ResponseBase<bool>> UpdateInvoiceStatus([FromQuery] int invoiceId, [FromQuery] string status, [FromQuery] DateOnly? paidDate)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _invoiceRepository.UpdateInvoiceStatusAsync(invoiceId, status, paidDate);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
