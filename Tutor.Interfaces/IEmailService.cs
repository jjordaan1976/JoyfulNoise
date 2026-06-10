namespace Tutor.Data.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMagicLinkAsync(string recipientEmail, string recipientName, string magicLinkUrl);
        Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent);
    }
}
