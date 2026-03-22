using Microsoft.AspNetCore.Mvc;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Api
{
    [Route("Payment")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentRepository paymentRepository,
            ILogger<PaymentController> logger)
        {
            _paymentRepository = paymentRepository;
            _logger            = logger;
        }

        /// <summary>
        /// Returns all payments for the given account holder, newest first.
        /// </summary>
        [HttpGet("GetByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Payment>>> GetByAccountHolder(
            [FromQuery] int accountHolderId)
        {
            var result = await _paymentRepository.GetByAccountHolderAsync(accountHolderId);
            return new ResponseBase<IEnumerable<Payment>>
            {
                ReturnCode    = 0,
                ReturnMessage = "Success",
                Data          = result
            };
        }

        /// <summary>
        /// Records a manual payment and runs the allocation engine.
        /// Returns the new PaymentID.
        /// </summary>
        [HttpPost("Add")]
        public async Task<ResponseBase<int?>> Add([FromBody] Payment payment)
        {
            var newId = await _paymentRepository.AddPaymentAsync(payment);
            return new ResponseBase<int?>
            {
                ReturnCode    = newId.HasValue ? 0 : -1,
                ReturnMessage = newId.HasValue ? "Success" : "Failed to record payment",
                Data          = newId
            };
        }

        /// <summary>
        /// Creates a payment exactly equal to the invoice amount and marks it paid.
        /// Called when the teacher clicks the "Paid" button on an invoice row.
        /// </summary>
        [HttpPost("QuickPay")]
        public async Task<ResponseBase<int?>> QuickPay(
            [FromQuery] int invoiceId,
            [FromQuery] DateTime paymentDate)
        {
            var newId = await _paymentRepository.QuickPayInvoiceAsync(invoiceId, paymentDate);
            return new ResponseBase<int?>
            {
                ReturnCode    = newId.HasValue ? 0 : -1,
                ReturnMessage = newId.HasValue ? "Success" : "Failed to record quick-pay",
                Data          = newId
            };
        }

        /// <summary>
        /// Returns all PaymentAllocation rows for a given payment.
        /// </summary>
        [HttpGet("GetAllocations")]
        public async Task<ResponseBase<IEnumerable<PaymentAllocation>>> GetAllocations(
            [FromQuery] int paymentId)
        {
            var result = await _paymentRepository.GetAllocationsByPaymentAsync(paymentId);
            return new ResponseBase<IEnumerable<PaymentAllocation>>
            {
                ReturnCode    = 0,
                ReturnMessage = "Success",
                Data          = result
            };
        }
    }
}
