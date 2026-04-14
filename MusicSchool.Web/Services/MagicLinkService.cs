using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;

namespace MusicSchool.Services
{
    public class MagicLinkService(HttpClient http)
    {
        public async Task<string?> CreateForStudentAsync(int studentId)
        {
            try
            {
                var r = await http.PostAsync($"MagicLink/CreateForStudent?studentId={studentId}", null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<string>>();
                return result?.ReturnCode == 0 ? result.Data : null;
            }
            catch { return null; }
        }

        public async Task<string?> CreateForAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.PostAsync($"MagicLink/CreateForAccountHolder?accountHolderId={accountHolderId}", null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<string>>();
                return result?.ReturnCode == 0 ? result.Data : null;
            }
            catch { return null; }
        }
    }
}
