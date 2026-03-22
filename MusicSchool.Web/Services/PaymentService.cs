using MusicSchool.Data.Models;
using System.Net.Http.Json;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Services
{
    /// <summary>
    /// Client-side service for the Payment API endpoints.
    /// Register in Program.cs: builder.Services.AddScoped<PaymentService>();
    /// </summary>
    public class PaymentService
    {
        private readonly HttpClient _http;

        public PaymentService(HttpClient http) => _http = http;

        private async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ResponseBase<T>>(url);
                return response is not null ? response.Data : default;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[PaymentService.GetAsync] {url} — {ex.Message}");
                return default;
            }
        }

        private async Task<T?> PostAsync<T>(string url, object body)
        {
            try
            {
                var httpResponse = await _http.PostAsJsonAsync(url, body);
                httpResponse.EnsureSuccessStatusCode();
                var response = await httpResponse.Content.ReadFromJsonAsync<ResponseBase<T>>();
                return response is not null ? response.Data : default;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[PaymentService.PostAsync] {url} — {ex.Message}");
                return default;
            }
        }

        /// <summary>Returns all payments for an account holder, newest first.</summary>
        public async Task<List<Payment>> GetByAccountHolderAsync(int accountHolderId)
        {
            var result = await GetAsync<IEnumerable<Payment>>(
                $"Payment/GetByAccountHolder?accountHolderId={accountHolderId}");
            return result?.ToList() ?? [];
        }

        /// <summary>
        /// Records a manual payment and runs the allocation engine.
        /// Returns the new PaymentID on success, null on failure.
        /// </summary>
        public async Task<int?> AddPaymentAsync(Payment payment)
            => await PostAsync<int?>("Payment/Add", payment);

        /// <summary>
        /// Creates a payment exactly equal to the invoice amount and marks it paid.
        /// Returns the new PaymentID on success, null on failure.
        /// </summary>
        public async Task<int?> QuickPayInvoiceAsync(int invoiceId, DateTime paymentDate)
        {
            var url = $"Payment/QuickPay?invoiceId={invoiceId}&paymentDate={paymentDate:yyyy-MM-dd}";
            return await PostAsync<int?>(url, new { });
        }

        /// <summary>Returns allocation detail rows for a specific payment.</summary>
        public async Task<List<PaymentAllocation>> GetAllocationsAsync(int paymentId)
        {
            var result = await GetAsync<IEnumerable<PaymentAllocation>>(
                $"Payment/GetAllocations?paymentId={paymentId}");
            return result?.ToList() ?? [];
        }
    }
}
