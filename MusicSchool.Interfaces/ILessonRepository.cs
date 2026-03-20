using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonRepository
    {
        Task<LessonDetail?> GetLessonAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<int?> AddLessonAsync(Lesson lesson);

        /// <summary>
        /// Updates the lesson status, keeps BundleQuarter.LessonsUsed in sync,
        /// and optionally records a free-text <paramref name="note"/>.
        /// When <paramref name="note"/> is null the existing Notes value is preserved.
        /// </summary>
        Task<bool> UpdateLessonStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null);
    }
}
