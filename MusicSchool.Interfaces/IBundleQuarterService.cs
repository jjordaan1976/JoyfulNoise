using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IBundleQuarterService
    {
        Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId);
        Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx);
        Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed);
    }
}
