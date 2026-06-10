using Tutor.Data.Models;
using Tutor.Models;
using System.Data;

namespace Tutor.Data.Interfaces
{
    public interface ILessonBundleDataAccessObject
    {
        Task<LessonBundle?> GetBundleAsync(int id);
        Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId);
        Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx);
        Task<bool> UpdateAsync(LessonBundle bundle);
    }
}
