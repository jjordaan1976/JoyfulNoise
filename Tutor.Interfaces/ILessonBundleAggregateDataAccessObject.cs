using Tutor.Data.Models;
using Tutor.Models;

namespace Tutor.Data.Interfaces
{
    public interface ILessonBundleAggregateDataAccessObject
    {
        Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleByIdAsync(int bundleId);
        Task<IEnumerable<LessonBundleDetail>> GetBundleByStudentIdAsync(int bundleId);
    }
}
