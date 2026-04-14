using MusicSchool.Data.Models;
using MusicSchool.Models;
using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;

namespace MusicSchool.StudentPortal.Services;

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

    public Task<Student?> GetStudentAsync()
        => GetAsync<Student>("StudentPortal/GetStudent");

    public async Task<List<LessonBundleDetail>> GetBundlesAsync()
    {
        var result = await GetAsync<IEnumerable<LessonBundleDetail>>("StudentPortal/GetBundles");
        return result?.ToList() ?? [];
    }

    public async Task<List<ScheduledSlot>> GetActiveSlotsAsync()
    {
        var result = await GetAsync<IEnumerable<ScheduledSlot>>("StudentPortal/GetSlots");
        return result?.ToList() ?? [];
    }

    public async Task<List<Lesson>> GetLessonsByBundleAsync(int bundleId)
    {
        var result = await GetAsync<IEnumerable<Lesson>>($"StudentPortal/GetLessonsByBundle?bundleId={bundleId}");
        return result?.ToList() ?? [];
    }
}
