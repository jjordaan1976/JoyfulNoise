using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [Route("AccountHolderPortal")]
    public class AccountHolderPortalController : BaseController
    {
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<AccountHolderPortalController> _logger;

        public AccountHolderPortalController(
            IAccountHolderRepository accountHolderRepository,
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUser,
            ILogger<AccountHolderPortalController> logger)
        {
            _accountHolderRepository = accountHolderRepository;
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        [HttpGet("GetAccountHolder")]
        public Task<ResponseBase<Tutor.Data.Models.AccountHolder?>> GetAccountHolder()
            => Execute(() => _accountHolderRepository.GetAccountHolderAsync(_currentUser.RequireAccountHolderId()), _logger, "Error getting account holder");

        [HttpGet("GetAllInvoices")]
        public Task<ResponseBase<IEnumerable<Tutor.Data.Models.Invoice>>> GetAllInvoices()
            => Execute(() => _invoiceRepository.GetByAccountHolderAsync(_currentUser.RequireAccountHolderId()), _logger, "Error getting invoices");

        [HttpGet("GetOutstandingInvoices")]
        public Task<ResponseBase<IEnumerable<Tutor.Data.Models.Invoice>>> GetOutstandingInvoices()
            => Execute(() => _invoiceRepository.GetOutstandingByAccountHolderAsync(_currentUser.RequireAccountHolderId()), _logger, "Error getting outstanding invoices");

        [HttpGet("GetPayments")]
        public Task<ResponseBase<IEnumerable<Tutor.Data.Models.Payment>>> GetPayments()
            => Execute(() => _paymentRepository.GetByAccountHolderAsync(_currentUser.RequireAccountHolderId()), _logger, "Error getting payments");
    }
}
