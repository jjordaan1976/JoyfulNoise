using MusicSchool.Data.Models;
using MusicSchool.Models;
using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;


namespace MusicSchool.StudentPortal.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    // ── Generic helper ───────────────────────────────────────────
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

    // ── Student ──────────────────────────────────────────────────
    public Task<Student?> GetStudentAsync(int studentId)
        => GetAsync<Student>($"Student/GetStudent?id={studentId}");

    // ── Lessons ──────────────────────────────────────────────────
    /// <summary>
    /// Returns all lessons for a bundle (base Lesson model).
    /// We fetch bundles first, then pull lessons per bundle.
    /// </summary>
    public async Task<List<Lesson>> GetLessonsByBundleAsync(int bundleId)
    {
        var result = await GetAsync<IEnumerable<Lesson>>(
            $"Lesson/GetByBundle?bundleId={bundleId}");
        return result?.ToList() ?? [];
    }

    // ── Bundles ──────────────────────────────────────────────────
    public async Task<List<LessonBundleDetail>> GetBundlesByStudentAsync(int studentId)
    {
        var result = await GetAsync<IEnumerable<LessonBundleDetail>>(
            $"LessonBundle/GetByStudent?studentId={studentId}");
        return result?.ToList() ?? [];
    }

    // ── Scheduled slots ──────────────────────────────────────────
    public async Task<List<ScheduledSlot>> GetActiveSlotsByStudentAsync(int studentId)
    {
        var result = await GetAsync<IEnumerable<ScheduledSlot>>(
            $"ScheduledSlot/GetActiveByStudent?studentId={studentId}");
        return result?.ToList() ?? [];
    }
}
