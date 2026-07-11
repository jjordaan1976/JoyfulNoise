namespace Tutor.Data.Interfaces
{
    public interface IAuthRepository
    {
        /// <summary>Generates, stores and emails a one-time password. Throws on failure.</summary>
        Task<bool> RequestOtpAsync(string email);

        /// <summary>Validates the OTP and returns a JWT identifying the user. Throws on failure.</summary>
        Task<string> VerifyOtpAsync(string email, string otp);
    }
}
