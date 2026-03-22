using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IBundleQuarterDataAccessObject
    {
        Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId);

        /// <summary>
        /// Inserts a batch of quarters within an existing transaction.
        /// The connection must be passed explicitly so the INSERT runs on the
        /// same connection that owns the transaction.
        /// </summary>
        Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx, IDbConnection connection);

        Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed);

        /// <summary>
        /// Atomically increments or decrements LessonsUsed for the quarter that
        /// owns <paramref name="lessonId"/> by <paramref name="delta"/> (+1 or -1).
        /// Uses a single UPDATE … SET LessonsUsed = LessonsUsed + @Delta to avoid
        /// read-then-write races. Clamps to zero so LessonsUsed never goes negative.
        /// </summary>
        Task<bool> AdjustLessonsUsedAsync(int lessonId, int delta);
    }
}
