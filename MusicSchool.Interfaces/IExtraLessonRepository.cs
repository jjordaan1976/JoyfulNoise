using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonRepository
    {
        Task<ExtraLessonDetail?> GetExtraLessonAsync(int extraLessonId);
        Task<IEnumerable<ExtraLessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically in a single transaction.
        /// Returns the new ExtraLessonID, or null if the operation fails.
        /// </summary>
        Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson);

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status, string? note = null);
    }
}
