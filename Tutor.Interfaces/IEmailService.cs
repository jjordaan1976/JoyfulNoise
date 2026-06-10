namespace Tutor.Data.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent);
    }
}
