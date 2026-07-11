using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface ILessonRepository
    {
        Task<LessonDetail?> GetLessonAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);

        /// <summary>
        /// Returns the lessons for a bundle, but only if the bundle belongs to the
        /// given student. Throws InvalidOperationException otherwise.
        /// </summary>
        Task<IEnumerable<Lesson>> GetByBundleForStudentAsync(int bundleId, int studentId);
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
