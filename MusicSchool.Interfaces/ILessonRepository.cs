using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonRepository
    {
        Task<LessonDetail?> GetLessonAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<int?> AddLessonAsync(Lesson lesson);
        Task<bool> UpdateLessonStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt);
    }
}
