using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;

namespace MusicSchool.AccountHolderPortal.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ResponseBase<T>>(url);
            return response is not null ? response.Data : default;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ApiService.GetAsync] {url} — {ex.Message}");
            return default;
        }
    }

    // ── Convenience helpers ──────────────────────────────────────

    public Task<AccountHolder?> GetAccountHolderAsync(int id)
        => GetAsync<AccountHolder>($"AccountHolder/GetAccountHolder?id={id}");

    public async Task<List<Invoice>> GetAllInvoicesAsync(int accountHolderId)
    {
        var result = await GetAsync<IEnumerable<Invoice>>(
            $"Invoice/GetByAccountHolder?accountHolderId={accountHolderId}");
        return result?.ToList() ?? [];
    }

    public async Task<List<Invoice>> GetOutstandingInvoicesAsync(int accountHolderId)
    {
        var result = await GetAsync<IEnumerable<Invoice>>(
            $"Invoice/GetOutstandingByAccountHolder?accountHolderId={accountHolderId}");
        return result?.ToList() ?? [];
    }

    public async Task<List<Payment>> GetPaymentsByAccountHolderAsync(int accountHolderId)
{
    var result = await GetAsync<IEnumerable<Payment>>(
        $"Payment/GetByAccountHolder?accountHolderId={accountHolderId}");
    return result?.ToList() ?? [];
}
}
