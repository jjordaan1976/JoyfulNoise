using System.Net.Http.Json;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Services
{
    public class TeacherService(HttpClient http)
    {
        public async Task<List<Teacher>> GetAllActiveAsync()
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Teacher>>>("Teacher/GetAllActive");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<Teacher>>($"Teacher/GetTeacher?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddTeacherAsync(Teacher teacher)
        {
            try
            {
                var r = await http.PostAsJsonAsync("Teacher/AddTeacher", teacher);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                var r = await http.PutAsJsonAsync("Teacher/UpdateTeacher", teacher);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class AccountHolderService(HttpClient http)
    {
        public async Task<List<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<AccountHolder>>>($"AccountHolder/GetByTeacher?teacherId={teacherId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<AccountHolder>>($"AccountHolder/GetAccountHolder?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                var r = await http.PostAsJsonAsync("AccountHolder/AddAccountHolder", accountHolder);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                var r = await http.PutAsJsonAsync("AccountHolder/UpdateAccountHolder", accountHolder);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class StudentService(HttpClient http)
    {
        public async Task<List<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Student>>>($"Student/GetByAccountHolder?accountHolderId={accountHolderId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<Student>>($"Student/GetStudent?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            try
            {
                var r = await http.PostAsJsonAsync("Student/AddStudent", student);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                var r = await http.PutAsJsonAsync("Student/UpdateStudent", student);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class LessonTypeService(HttpClient http)
    {
        public async Task<List<LessonType>> GetAllActiveAsync()
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonType>>>("LessonType/GetAllActive");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<LessonType>>($"LessonType/GetLessonType?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                var r = await http.PostAsJsonAsync("LessonType/AddLessonType", lessonType);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                var r = await http.PutAsJsonAsync("LessonType/UpdateLessonType", lessonType);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class LessonBundleService(HttpClient http)
    {
        public async Task<List<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonBundleWithQuarterDetail>>>($"LessonBundle/GetBundle?bundleId={bundleId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<LessonBundleDetail>> GetByStudentAsync(int studentId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonBundleDetail>>>($"LessonBundle/GetByStudent?studentId={studentId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<int?> AddBundleAsync(AddBundleRequest request)
        {
            try
            {
                var r = await http.PostAsJsonAsync("LessonBundle/AddBundle", request);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            try
            {
                var r = await http.PutAsJsonAsync("LessonBundle/UpdateBundle", bundle);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class ScheduledSlotService(HttpClient http)
    {
        public async Task<List<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ScheduledSlot>>>($"ScheduledSlot/GetActiveByStudent?studentId={studentId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ScheduledSlot>>>($"ScheduledSlot/GetActiveByTeacher?teacherId={teacherId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<ScheduledSlot>>($"ScheduledSlot/GetSlot?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        /// <summary>
        /// Returns (newSlotId, null) on success, or (null, errorMessage) when the API
        /// rejects the request (e.g. no active bundle with remaining credits).
        /// </summary>
        public async Task<(int? Id, string? Error)> AddSlotAsync(ScheduledSlot slot)
        {
            try
            {
                var r = await http.PostAsJsonAsync("ScheduledSlot/AddSlot", slot);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                if (result is null) return (null, "Unexpected error communicating with the server.");
                if (result.ReturnCode != 0) return (null, result.ReturnMessage);
                return (result.Data, null);
            }
            catch (Exception ex) { return (null, ex.Message); }
        }

        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            try
            {
                var r = await http.PutAsync($"ScheduledSlot/CloseSlot?slotId={slotId}&effectiveTo={effectiveTo:yyyy-MM-dd}", null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class LessonService(HttpClient http)
    {
        public async Task<List<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime date)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonDetail>>>($"Lesson/GetByTeacherAndDate?teacherId={teacherId}&scheduledDate={date:yyyy-MM-dd}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<Lesson>> GetByBundleAsync(int bundleId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Lesson>>>($"Lesson/GetByBundle?bundleId={bundleId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<LessonDetail>>($"Lesson/GetLesson?lessonId={lessonId}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddLessonAsync(Lesson lesson)
        {
            try
            {
                var r = await http.PostAsJsonAsync("Lesson/AddLesson", lesson);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status, string? note = null)
        {
            try
            {
                var url = $"Lesson/UpdateLessonStatus?lessonId={lessonId}&status={status}";
                if (!string.IsNullOrWhiteSpace(note))
                    url += $"&note={Uri.EscapeDataString(note)}";
                var r = await http.PutAsync(url, null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class ExtraLessonService(HttpClient http)
    {
        public async Task<List<ExtraLessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime date)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ExtraLessonDetail>>>($"ExtraLesson/GetByTeacherAndDate?teacherId={teacherId}&scheduledDate={date:yyyy-MM-dd}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ExtraLesson>>>($"ExtraLesson/GetByStudent?studentId={studentId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<int?> AddExtraLessonAsync(ExtraLesson lesson)
        {
            try
            {
                var r = await http.PostAsJsonAsync("ExtraLesson/AddExtraLesson", lesson);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status, string? note = null)
        {
            try
            {
                var url = $"ExtraLesson/UpdateExtraLessonStatus?extraLessonId={extraLessonId}&status={status}";
                if (!string.IsNullOrWhiteSpace(note))
                    url += $"&note={Uri.EscapeDataString(note)}";
                var r = await http.PutAsync(url, null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class InvoiceService(HttpClient http)
    {
        public async Task<List<Invoice>> GetByBundleAsync(int bundleId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetByBundle?bundleId={bundleId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetByAccountHolder?accountHolderId={accountHolderId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetOutstandingByAccountHolder?accountHolderId={accountHolderId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            try
            {
                var url = $"Invoice/UpdateInvoiceStatus?invoiceId={invoiceId}&status={status}";
                if (paidDate.HasValue) url += $"&paidDate={paidDate.Value:yyyy-MM-dd}";
                var r = await http.PutAsync(url, null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }
}
