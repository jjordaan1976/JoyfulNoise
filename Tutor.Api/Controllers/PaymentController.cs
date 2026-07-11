using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Api
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
        public Task<ResponseBase<IEnumerable<Payment>>> GetByAccountHolder([FromQuery] int accountHolderId)
            => Execute(() => _paymentRepository.GetByAccountHolderAsync(accountHolderId), _logger, "Error getting payments by account holder");

        /// <summary>
        /// Records a manual payment and runs the allocation engine.
        /// Returns the new PaymentID.
        /// </summary>
        [HttpPost("Add")]
        public Task<ResponseBase<int?>> Add([FromBody] Payment payment)
            => Execute(() => _paymentRepository.AddPaymentAsync(payment), _logger, "Error recording payment");

        /// <summary>
        /// Creates a payment exactly equal to the invoice amount and marks it paid.
        /// Called when the teacher clicks the "Paid" button on an invoice row.
        /// </summary>
        [HttpPost("QuickPay")]
        public Task<ResponseBase<int?>> QuickPay(
            [FromQuery] int invoiceId,
            [FromQuery] DateTime paymentDate)
            => Execute(() => _paymentRepository.QuickPayInvoiceAsync(invoiceId, paymentDate), _logger, "Error recording quick-pay");

        /// <summary>
        /// Returns all PaymentAllocation rows for a given payment.
        /// </summary>
        [HttpGet("GetAllocations")]
        public Task<ResponseBase<IEnumerable<PaymentAllocation>>> GetAllocations([FromQuery] int paymentId)
            => Execute(() => _paymentRepository.GetAllocationsByPaymentAsync(paymentId), _logger, "Error getting payment allocations");
    }
}
