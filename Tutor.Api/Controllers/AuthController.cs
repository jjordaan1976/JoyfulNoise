using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.JwtService;
using Tutor.Models;
using Tutor.Repositories;

namespace Tutor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IJwtTokenService _jwtService;
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly IStudentRepository _studentRepository;

        public AuthController(
            IEmailService emailService,
            IOtpService otpService,
            IJwtTokenService jwtService,
            IAccountHolderRepository accountHolderRepository,
            IStudentRepository studentRepository)
        {
            _emailService = emailService;
            _otpService = otpService;
            _jwtService = jwtService;
            _accountHolderRepository = accountHolderRepository;
            _studentRepository = studentRepository;
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new OtpResponse { Success = false, Message = "Email is required" });

            // Generate OTP
            var otp = _otpService.GenerateOtp();

            // Store OTP
            _otpService.StoreOtp(request.Email, otp);

            // Send via Brevo
            var subject = "Your OTP Code";
            var htmlContent = $@"
                <h2>Your One-Time Password</h2>
                <p>Your OTP code is: <strong style='font-size: 24px; letter-spacing: 2px;'>{otp}</strong></p>
                <p>This code expires in 10 minutes.</p>
                <p>Do not share this code with anyone.</p>";

            try
            {
                await _emailService.SendAsync(request.Email, "", subject, htmlContent);
                return Ok(new OtpResponse { Success = true, Message = "OTP sent to your email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new OtpResponse { Success = false, Message = $"Failed to send OTP: {ex.Message}" });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest(new TokenResponse { Success = false, Message = "Email and OTP are required" });

            // Validate OTP
            if (!_otpService.ValidateOtp(request.Email, request.Otp))
                return Unauthorized(new TokenResponse { Success = false, Message = "Invalid or expired OTP" });

            // Generate JWT
            var claims = new Dictionary<string, string>();

            // Try to find user (AccountHolder or Student)
            var accountHolder = await _accountHolderRepository.GetByEmailAsync(request.Email);
            if (accountHolder != null)
            {
                claims["accountHolderId"] = accountHolder.AccountHolderID.ToString();
                claims["role"] = "AccountHolder";
            }
            else
            {
                var student = await _studentRepository.GetByEmailAsync(request.Email);
                if (student != null)
                {
                    claims["studentId"] = student.StudentID.ToString();
                    claims["role"] = "Student";
                }
            }

            var token = _jwtService.GenerateToken(request.Email, claims);

            return Ok(new TokenResponse { Success = true, Token = token, Message = "Authentication successful" });
        }
    }
}
