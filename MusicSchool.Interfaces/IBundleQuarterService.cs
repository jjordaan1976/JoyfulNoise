using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IBundleQuarterService
    {
        Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId);
        Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx);
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
