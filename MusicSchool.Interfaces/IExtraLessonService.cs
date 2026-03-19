using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonService
    {
        Task<ExtraLesson?> GetExtraLessonAsync(int id);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);
        Task<int> InsertAsync(ExtraLesson extraLesson);
        Task<bool> UpdateStatusAsync(int extraLessonId, string status);
    }
}
