using Tutor.Data.Models;
using Tutor.Models;

namespace Tutor.Data.Interfaces
{
    public interface ILessonBundleRepository
    {
        Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId);
        Task<IEnumerable<LessonBundleDetail>> GetByStudentAsync(int studentId);
        Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters, int selectedBundleLessons);
        Task<bool> UpdateBundleAsync(LessonBundle bundle);
    }
}
