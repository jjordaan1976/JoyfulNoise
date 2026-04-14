using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;

namespace MusicSchool.AccountHolderPortal.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    private async Task<T?> GetAsync<T>(string url)
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

    public Task<AccountHolder?> GetAccountHolderAsync()
        => GetAsync<AccountHolder>("AccountHolderPortal/GetAccountHolder");

    public async Task<List<Invoice>> GetAllInvoicesAsync()
    {
        var result = await GetAsync<IEnumerable<Invoice>>("AccountHolderPortal/GetAllInvoices");
        return result?.ToList() ?? [];
    }

    public async Task<List<Invoice>> GetOutstandingInvoicesAsync()
    {
        var result = await GetAsync<IEnumerable<Invoice>>("AccountHolderPortal/GetOutstandingInvoices");
        return result?.ToList() ?? [];
    }

    public async Task<List<Payment>> GetPaymentsByAccountHolderAsync()
    {
        var result = await GetAsync<IEnumerable<Payment>>("AccountHolderPortal/GetPayments");
        return result?.ToList() ?? [];
    }
}
