namespace Tutor.Models
{
    public class OtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class OtpVerifyRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class OtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
