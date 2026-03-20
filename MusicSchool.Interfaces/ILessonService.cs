using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonService
    {
        Task<Lesson?> GetLessonAsync(int id);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Lesson>> GetByStatusAsync(string status);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(Lesson lesson);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(Lesson lesson, IDbTransaction tx);

        /// <summary>
        /// Updates the status fields on a lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null);
    }
}
