using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [Route("MagicLink")]
    public class MagicLinkController : BaseController
    {
        private readonly IMagicLinkRepository _magicLinkRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<MagicLinkController> _logger;

        public MagicLinkController(IMagicLinkRepository magicLinkRepository, IEmailService emailService, ILogger<MagicLinkController> logger)
        {
            _magicLinkRepository = magicLinkRepository;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("CreateForStudent")]
        public async Task<ResponseBase<string>> CreateForStudent([FromQuery] int studentId, [FromQuery] string? email, [FromQuery] string? recipientName)
        {
            var response = new ResponseBase<string> { ReturnCode = -1 };
            var token = await _magicLinkRepository.CreateForStudentAsync(studentId);
            if (token is null)
            {
                response.ReturnMessage = "Failed to create magic link.";
                return response;
            }

            // Send email if provided
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(recipientName))
            {
                var magicLinkUrl = $"https://yourdomain.com/authenticate?token={token}";
                var emailSent = await _emailService.SendMagicLinkAsync(email, recipientName, magicLinkUrl);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send magic link email to {Email}", email);
                }
            }

            response.Data = token.Value.ToString();
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("CreateForAccountHolder")]
        public async Task<ResponseBase<string>> CreateForAccountHolder([FromQuery] int accountHolderId, [FromQuery] string? email, [FromQuery] string? recipientName)
        {
            var response = new ResponseBase<string> { ReturnCode = -1 };
            var token = await _magicLinkRepository.CreateForAccountHolderAsync(accountHolderId);
            if (token is null)
            {
                response.ReturnMessage = "Failed to create magic link.";
                return response;
            }

            // Send email if provided
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(recipientName))
            {
                var magicLinkUrl = $"https://yourdomain.com/authenticate?token={token}";
                var emailSent = await _emailService.SendMagicLinkAsync(email, recipientName, magicLinkUrl);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send magic link email to {Email}", email);
                }
            }

            response.Data = token.Value.ToString();
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
