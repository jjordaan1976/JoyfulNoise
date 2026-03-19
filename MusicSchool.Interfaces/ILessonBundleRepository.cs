using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleRepository
    {
        Task<IEnumerable<LessonBundleDetail>> GetBundleAsync(int bundleId);
        Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId);
        Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<bool> UpdateBundleAsync(LessonBundle bundle);
    }
}
