using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonService
    {
        Task<ExtraLesson?> GetExtraLessonAsync(int id);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(ExtraLesson extraLesson);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection);

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        Task<bool> UpdateStatusAsync(int extraLessonId, string status, string? note = null);
    }
}
