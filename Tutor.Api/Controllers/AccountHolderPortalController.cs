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
        public async Task<ResponseBase<Tutor.Data.Models.AccountHolder>> GetAccountHolder()
        {
            var response = new ResponseBase<Tutor.Data.Models.AccountHolder> { ReturnCode = -1 };
            var accountHolderId = int.TryParse(GetClaimValue("accountHolderId"), out var id) ? id : 0;
            if (accountHolderId == 0)
            {
                response.ReturnMessage = "Account holder not found in token claims.";
                return response;
            }
            var result = await _accountHolderRepository.GetAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetAllInvoices")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.Invoice>>> GetAllInvoices()
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.Invoice>> { ReturnCode = -1 };
            var accountHolderId = int.TryParse(GetClaimValue("accountHolderId"), out var id) ? id : 0;
            if (accountHolderId == 0)
            {
                response.ReturnMessage = "Account holder not found in token claims.";
                return response;
            }
            var result = await _invoiceRepository.GetByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetOutstandingInvoices")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.Invoice>>> GetOutstandingInvoices()
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.Invoice>> { ReturnCode = -1 };
            var accountHolderId = int.TryParse(GetClaimValue("accountHolderId"), out var id) ? id : 0;
            if (accountHolderId == 0)
            {
                response.ReturnMessage = "Account holder not found in token claims.";
                return response;
            }
            var result = await _invoiceRepository.GetOutstandingByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetPayments")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.Payment>>> GetPayments()
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.Payment>> { ReturnCode = -1 };
            var accountHolderId = int.TryParse(GetClaimValue("accountHolderId"), out var id) ? id : 0;
            if (accountHolderId == 0)
            {
                response.ReturnMessage = "Account holder not found in token claims.";
                return response;
            }
            var result = await _paymentRepository.GetByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
