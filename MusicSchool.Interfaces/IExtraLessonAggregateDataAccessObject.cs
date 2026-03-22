using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonAggregateDataAccessObject
    {
        Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId);

        Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate);

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically.
        /// Returns the new ExtraLessonID.
        /// Throws <see cref="InvalidOperationException"/> when the student is not found.
        /// </summary>
        Task<int> SaveNewExtraLessonAsync(ExtraLesson extraLesson);
    }
}
