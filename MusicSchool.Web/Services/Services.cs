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
            var r = await http.GetFromJsonAsync<ResponseBase<List<Teacher>>>("Teacher/GetAllActive");
            return r?.Data ?? [];
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<Teacher>>($"Teacher/GetTeacher?id={id}");
            return r?.Data;
        }

        public async Task<int?> AddTeacherAsync(Teacher teacher)
        {
            var r = await http.PostAsJsonAsync("Teacher/AddTeacher", teacher);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            var r = await http.PutAsJsonAsync("Teacher/UpdateTeacher", teacher);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class AccountHolderService(HttpClient http)
    {
        public async Task<List<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<AccountHolder>>>($"AccountHolder/GetByTeacher?teacherId={teacherId}");
            return r?.Data ?? [];
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<AccountHolder>>($"AccountHolder/GetAccountHolder?id={id}");
            return r?.Data;
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            var r = await http.PostAsJsonAsync("AccountHolder/AddAccountHolder", accountHolder);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            var r = await http.PutAsJsonAsync("AccountHolder/UpdateAccountHolder", accountHolder);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class StudentService(HttpClient http)
    {
        public async Task<List<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<Student>>>($"Student/GetByAccountHolder?accountHolderId={accountHolderId}");
            return r?.Data ?? [];
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<Student>>($"Student/GetStudent?id={id}");
            return r?.Data;
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            var r = await http.PostAsJsonAsync("Student/AddStudent", student);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            var r = await http.PutAsJsonAsync("Student/UpdateStudent", student);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class LessonTypeService(HttpClient http)
    {
        public async Task<List<LessonType>> GetAllActiveAsync()
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<LessonType>>>("LessonType/GetAllActive");
            return r?.Data ?? [];
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<LessonType>>($"LessonType/GetLessonType?id={id}");
            return r?.Data;
        }

        public async Task<int?> AddLessonTypeAsync(LessonType lessonType)
        {
            var r = await http.PostAsJsonAsync("LessonType/AddLessonType", lessonType);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            var r = await http.PutAsJsonAsync("LessonType/UpdateLessonType", lessonType);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class LessonBundleService(HttpClient http)
    {
        public async Task<List<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<LessonBundleWithQuarterDetail>>>($"LessonBundle/GetBundle?bundleId={bundleId}");
            return r?.Data ?? [];
        }

        public async Task<List<LessonBundleDetail>> GetByStudentAsync(int studentId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<LessonBundleDetail>>>($"LessonBundle/GetByStudent?studentId={studentId}");
            return r?.Data ?? [];
        }

        public async Task<int?> AddBundleAsync(AddBundleRequest request)
        {
            var r = await http.PostAsJsonAsync("LessonBundle/AddBundle", request);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            var r = await http.PutAsJsonAsync("LessonBundle/UpdateBundle", bundle);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class ScheduledSlotService(HttpClient http)
    {
        public async Task<List<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<ScheduledSlot>>>($"ScheduledSlot/GetActiveByStudent?studentId={studentId}");
            return r?.Data ?? [];
        }

        public async Task<List<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<ScheduledSlot>>>($"ScheduledSlot/GetActiveByTeacher?teacherId={teacherId}");
            return r?.Data ?? [];
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<ScheduledSlot>>($"ScheduledSlot/GetSlot?id={id}");
            return r?.Data;
        }

        public async Task<int?> AddSlotAsync(ScheduledSlot slot)
        {
            var r = await http.PostAsJsonAsync("ScheduledSlot/AddSlot", slot);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            var r = await http.PutAsync($"ScheduledSlot/CloseSlot?slotId={slotId}&effectiveTo={effectiveTo:yyyy-MM-dd}", null);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class LessonService(HttpClient http)
    {
        public async Task<List<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateOnly date)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<LessonDetail>>>($"Lesson/GetByTeacherAndDate?teacherId={teacherId}&scheduledDate={date:yyyy-MM-dd}");
            return r?.Data ?? [];
        }

        public async Task<List<LessonDetail>> GetByStudentAsync(int studentId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<LessonDetail>>>($"Lesson/GetByStudent?studentId={studentId}");
            return r?.Data ?? [];
        }

        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<LessonDetail>>($"Lesson/GetLesson?lessonId={lessonId}");
            return r?.Data;
        }

        public async Task<int?> AddLessonAsync(Lesson lesson)
        {
            var r = await http.PostAsJsonAsync("Lesson/AddLesson", lesson);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status)
        {
            var r = await http.PutAsync($"Lesson/UpdateLessonStatus?lessonId={lessonId}&status={status}", null);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class ExtraLessonService(HttpClient http)
    {
        public async Task<List<ExtraLessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateOnly date)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<ExtraLessonDetail>>>($"ExtraLesson/GetByTeacherAndDate?teacherId={teacherId}&scheduledDate={date:yyyy-MM-dd}");
            return r?.Data ?? [];
        }

        public async Task<List<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<ExtraLesson>>>($"ExtraLesson/GetByStudent?studentId={studentId}");
            return r?.Data ?? [];
        }

        public async Task<int?> AddExtraLessonAsync(ExtraLesson lesson)
        {
            var r = await http.PostAsJsonAsync("ExtraLesson/AddExtraLesson", lesson);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
            return result?.Data;
        }

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status)
        {
            var r = await http.PutAsync($"ExtraLesson/UpdateExtraLessonStatus?extraLessonId={extraLessonId}&status={status}", null);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }

    public class InvoiceService(HttpClient http)
    {
        public async Task<List<Invoice>> GetByBundleAsync(int bundleId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetByBundle?bundleId={bundleId}");
            return r?.Data ?? [];
        }

        public async Task<List<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetByAccountHolder?accountHolderId={accountHolderId}");
            return r?.Data ?? [];
        }

        public async Task<List<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetOutstandingByAccountHolder?accountHolderId={accountHolderId}");
            return r?.Data ?? [];
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            var url = $"Invoice/UpdateInvoiceStatus?invoiceId={invoiceId}&status={status}";
            if (paidDate.HasValue) url += $"&paidDate={paidDate.Value:yyyy-MM-dd}";
            var r = await http.PutAsync(url, null);
            var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
            return result?.Data ?? false;
        }
    }
}
