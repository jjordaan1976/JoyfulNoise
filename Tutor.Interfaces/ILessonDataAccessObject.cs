using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Interfaces
{
    public interface ILessonDataAccessObject
    {
        Task<Lesson?> GetLessonAsync(int id);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Lesson>> GetByStatusAsync(string status);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(Lesson lesson);

        /// <summary>
        /// Inserts within an existing transaction. Executes against the passed-in
        /// connection (the one that owns the transaction).
        /// </summary>
        Task<int> InsertAsync(Lesson lesson, IDbConnection connection, IDbTransaction transaction);

        Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null);

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled,
        /// clearing CancelledBy, CancellationReason and CreditForfeited.
        /// </summary>
        Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime);
    }
}
