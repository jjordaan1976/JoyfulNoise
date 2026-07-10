using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;

namespace Tutor.Data.Implementations
{
    /// <summary>
    /// Development-only IEmailService that prints the email to the API console instead of sending it.
    /// </summary>
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent)
        {
            _logger.LogInformation(
                "===== MOCK EMAIL =====\nTo: {ToEmail}\nSubject: {Subject}\n{Content}\n=======================",
                toEmail, subject, htmlContent);
            return Task.FromResult(true);
        }
    }
}
