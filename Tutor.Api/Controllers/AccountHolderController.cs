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
        public Task<ResponseBase<AccountHolder?>> GetAccountHolder([FromQuery] int id)
            => Execute(() => _accountHolderRepository.GetAccountHolderAsync(id), _logger, "Error getting account holder");

        [HttpGet("GetByTeacher")]
        public Task<ResponseBase<IEnumerable<AccountHolder>>> GetByTeacher([FromQuery] int teacherId)
            => Execute(() => _accountHolderRepository.GetByTeacherAsync(teacherId), _logger, "Error getting account holders by teacher");

        [HttpPost("AddAccountHolder")]
        public Task<ResponseBase<int?>> AddAccountHolder([FromBody] AccountHolder req)
            => Execute(() => _accountHolderRepository.AddAccountHolderAsync(req), _logger, "Error adding account holder");

        [HttpPut("UpdateAccountHolder")]
        public Task<ResponseBase<bool>> UpdateAccountHolder([FromBody] AccountHolder req)
            => Execute(() => _accountHolderRepository.UpdateAccountHolderAsync(req), _logger, "Error updating account holder");
    }
}
