using Microsoft.AspNetCore.Mvc;
using MusicSchool.Data.Interfaces;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Api
{
    [Route("MagicLink")]
    public class MagicLinkController : BaseController
    {
        private readonly IMagicLinkRepository _magicLinkRepository;
        private readonly ILogger<MagicLinkController> _logger;

        public MagicLinkController(IMagicLinkRepository magicLinkRepository, ILogger<MagicLinkController> logger)
        {
            _magicLinkRepository = magicLinkRepository;
            _logger = logger;
        }

        [HttpPost("CreateForStudent")]
        public async Task<ResponseBase<string>> CreateForStudent([FromQuery] int studentId)
        {
            var response = new ResponseBase<string> { ReturnCode = -1 };
            var token = await _magicLinkRepository.CreateForStudentAsync(studentId);
            if (token is null)
            {
                response.ReturnMessage = "Failed to create magic link.";
                return response;
            }
            response.Data = token.Value.ToString();
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("CreateForAccountHolder")]
        public async Task<ResponseBase<string>> CreateForAccountHolder([FromQuery] int accountHolderId)
        {
            var response = new ResponseBase<string> { ReturnCode = -1 };
            var token = await _magicLinkRepository.CreateForAccountHolderAsync(accountHolderId);
            if (token is null)
            {
                response.ReturnMessage = "Failed to create magic link.";
                return response;
            }
            response.Data = token.Value.ToString();
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
