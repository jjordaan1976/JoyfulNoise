using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Tutor.Data.Interfaces;

namespace Tutor.Data.Implementations
{
    public class BrevoEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrevoEmailService> _logger;
        private const string BrevoApiUrl = "https://api.brevo.com/v3/smtp/email";

        public BrevoEmailService(IConfiguration configuration, HttpClient httpClient, ILogger<BrevoEmailService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent)
        {
            try
            {
                var apiKey = _configuration["Brevo:ApiKey"];
                var senderEmail = _configuration["Brevo:SenderEmail"];
                var senderName = _configuration["Brevo:SenderName"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogError("Brevo API key or sender email not configured");
                    return false;
                }

                var request = new BrevoEmailRequest
                {
                    Sender = new BrevoSender { Email = senderEmail, Name = senderName },
                    To = new[] { new BrevoRecipient { Email = toEmail, Name = toName } },
                    Subject = subject,
                    HtmlContent = htmlContent
                };

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, BrevoApiUrl);
                httpRequest.Headers.Add("api-key", apiKey);
                httpRequest.Content = JsonContent.Create(request);

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Brevo API error: {StatusCode} {Content}", response.StatusCode, errorContent);
                    return false;
                }

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via Brevo");
                return false;
            }
        }

        private class BrevoEmailRequest
        {
            [JsonPropertyName("sender")]
            public BrevoSender Sender { get; set; } = new();

            [JsonPropertyName("to")]
            public BrevoRecipient[] To { get; set; } = Array.Empty<BrevoRecipient>();

            [JsonPropertyName("subject")]
            public string Subject { get; set; } = "";

            [JsonPropertyName("htmlContent")]
            public string HtmlContent { get; set; } = "";
        }

        private class BrevoSender
        {
            [JsonPropertyName("email")]
            public string Email { get; set; } = "";

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
        }

        private class BrevoRecipient
        {
            [JsonPropertyName("email")]
            public string Email { get; set; } = "";

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
        }
    }
}
