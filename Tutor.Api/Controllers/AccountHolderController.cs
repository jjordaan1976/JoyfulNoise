using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [Route("AccountHolder")]
    public class AccountHolderController : BaseController
    {
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly ILogger<AccountHolderController> _logger;

        public AccountHolderController(ILogger<AccountHolderController> logger, IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
            _logger = logger;
        }

        [HttpGet("GetAccountHolder")]
        public async Task<ResponseBase<AccountHolder>> GetAccountHolder([FromQuery] int id)
        {
            ResponseBase<AccountHolder> response = new ResponseBase<AccountHolder>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.GetAccountHolderAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByTeacher")]
        public async Task<ResponseBase<IEnumerable<AccountHolder>>> GetByTeacher([FromQuery] int teacherId)
        {
            ResponseBase<IEnumerable<AccountHolder>> response = new ResponseBase<IEnumerable<AccountHolder>>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.GetByTeacherAsync(teacherId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddAccountHolder")]
        public async Task<ResponseBase<int?>> AddAccountHolder([FromBody] AccountHolder req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.AddAccountHolderAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateAccountHolder")]
        public async Task<ResponseBase<bool>> UpdateAccountHolder([FromBody] AccountHolder req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.UpdateAccountHolderAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
