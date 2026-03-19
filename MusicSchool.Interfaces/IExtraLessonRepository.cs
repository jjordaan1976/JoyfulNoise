using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonRepository
    {
        Task<ExtraLessonDetail?> GetExtraLessonAsync(int extraLessonId);
        Task<IEnumerable<ExtraLessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateOnly scheduledDate);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);
        Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson);
        Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status);
    }
}
