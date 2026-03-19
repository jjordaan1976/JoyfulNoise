using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleRepository
    {
        Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId);
        Task<IEnumerable<LessonBundleDetail>> GetByStudentAsync(int studentId);
        Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<bool> UpdateBundleAsync(LessonBundle bundle);
    }
}
