using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository authRepository, ILogger<AuthController> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        [HttpPost("request-otp")]
        public Task<ResponseBase<bool>> RequestOtp([FromBody] OtpRequest request)
            => Execute(() => _authRepository.RequestOtpAsync(request.Email), _logger, "Error requesting OTP");

        [HttpPost("verify-otp")]
        public Task<ResponseBase<string>> VerifyOtp([FromBody] OtpVerifyRequest request)
            => Execute(() => _authRepository.VerifyOtpAsync(request.Email, request.Otp), _logger, "Error verifying OTP");
    }
}
