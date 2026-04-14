using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api.Auth;
using MusicSchool.Data.Interfaces;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Api
{
    [Route("AccountHolderPortal")]
    [Authorize(AuthenticationSchemes = MagicLinkAuthenticationHandler.SchemeName)]
    [ApiController]
    public class AccountHolderPortalController : ControllerBase
    {
        private int AccountHolderId =>
            int.Parse(User.FindFirst(MagicLinkAuthenticationHandler.ClaimEntityId)!.Value);

        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;

        public AccountHolderPortalController(
            IAccountHolderRepository accountHolderRepository,
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository)
        {
            _accountHolderRepository = accountHolderRepository;
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
        }

        [HttpGet("GetAccountHolder")]
        public async Task<ResponseBase<MusicSchool.Data.Models.AccountHolder>> GetAccountHolder()
        {
            var response = new ResponseBase<MusicSchool.Data.Models.AccountHolder> { ReturnCode = -1 };
            var result = await _accountHolderRepository.GetAccountHolderAsync(AccountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetAllInvoices")]
        public async Task<ResponseBase<IEnumerable<MusicSchool.Data.Models.Invoice>>> GetAllInvoices()
        {
            var response = new ResponseBase<IEnumerable<MusicSchool.Data.Models.Invoice>> { ReturnCode = -1 };
            var result = await _invoiceRepository.GetByAccountHolderAsync(AccountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetOutstandingInvoices")]
        public async Task<ResponseBase<IEnumerable<MusicSchool.Data.Models.Invoice>>> GetOutstandingInvoices()
        {
            var response = new ResponseBase<IEnumerable<MusicSchool.Data.Models.Invoice>> { ReturnCode = -1 };
            var result = await _invoiceRepository.GetOutstandingByAccountHolderAsync(AccountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetPayments")]
        public async Task<ResponseBase<IEnumerable<MusicSchool.Data.Models.Payment>>> GetPayments()
        {
            var response = new ResponseBase<IEnumerable<MusicSchool.Data.Models.Payment>> { ReturnCode = -1 };
            var result = await _paymentRepository.GetByAccountHolderAsync(AccountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
