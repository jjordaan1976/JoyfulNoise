using Tutor.Data.Interfaces;
using Tutor.JwtService;
using Tutor.Repositories;

namespace Tutor.Data.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IJwtTokenService _jwtService;
        private readonly IUserRepository _userRepository;

        public AuthRepository(
            IEmailService emailService,
            IOtpService otpService,
            IJwtTokenService jwtService,
            IUserRepository userRepository)
        {
            _emailService = emailService;
            _otpService = otpService;
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<bool> RequestOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Email is required.");

            var otp = _otpService.GenerateOtp();
            _otpService.StoreOtp(email, otp);

            var htmlContent = $@"
                <h2>Your One-Time Password</h2>
                <p>Your OTP code is: <strong style='font-size: 24px; letter-spacing: 2px;'>{otp}</strong></p>
                <p>This code expires in 10 minutes.</p>
                <p>Do not share this code with anyone.</p>";

            var sent = await _emailService.SendAsync(email, "", "Your OTP Code", htmlContent);
            if (!sent)
                throw new InvalidOperationException("Failed to send the OTP email.");

            return true;
        }

        public async Task<string> VerifyOtpAsync(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                throw new InvalidOperationException("Email and OTP are required.");

            if (!_otpService.ValidateOtp(email, otp))
                throw new InvalidOperationException("Invalid or expired OTP.");

            // The token identifies the logged-in user. One email can hold several
            // roles (e.g. Student + AccountHolder); the token carries them all and
            // each portal reads the claim relevant to it.
            var users = (await _userRepository.GetByEmailAsync(email)).ToList();
            if (users.Count == 0)
                throw new InvalidOperationException("No user is registered for this email address.");

            var claims = new Dictionary<string, string>
            {
                ["name"] = users[0].DisplayName,
                ["role"] = string.Join(",", users.Select(u => u.Role.ToString()))
            };

            foreach (var user in users)
            {
                if (user.TeacherID.HasValue) claims["teacherId"] = user.TeacherID.Value.ToString();
                if (user.StudentID.HasValue) claims["studentId"] = user.StudentID.Value.ToString();
                if (user.AccountHolderID.HasValue) claims["accountHolderId"] = user.AccountHolderID.Value.ToString();
            }

            return _jwtService.GenerateToken(email, claims);
        }
    }
}
