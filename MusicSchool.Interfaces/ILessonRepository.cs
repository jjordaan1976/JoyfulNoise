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
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null);

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled.
        /// Only valid when the lesson's current status is CancelledTeacher or CancelledStudent.
        /// </summary>
        Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime);
    }
}
