using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleAggregateService
    {
        Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<IEnumerable<LessonBundleDetail>> GetBundleByIdAsync(int bundleId);
    }
}
