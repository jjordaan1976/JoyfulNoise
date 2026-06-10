namespace Tutor.Repositories
{
    public interface IOtpService
    {
        string GenerateOtp();
        bool StoreOtp(string email, string otp, int expiryMinutes = 10);
        bool ValidateOtp(string email, string otp);
    }

    public class OtpService : IOtpService
    {
        private readonly Dictionary<string, (string otp, DateTime expiry)> _otpStore = new();
        private readonly object _lockObject = new();

        public string GenerateOtp()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }

        public bool StoreOtp(string email, string otp, int expiryMinutes = 10)
        {
            lock (_lockObject)
            {
                _otpStore[email] = (otp, DateTime.UtcNow.AddMinutes(expiryMinutes));
                return true;
            }
        }

        public bool ValidateOtp(string email, string otp)
        {
            lock (_lockObject)
            {
                if (!_otpStore.TryGetValue(email, out var stored))
                    return false;

                if (DateTime.UtcNow > stored.expiry)
                {
                    _otpStore.Remove(email);
                    return false;
                }

                if (stored.otp != otp)
                    return false;

                _otpStore.Remove(email);
                return true;
            }
        }
    }
}
