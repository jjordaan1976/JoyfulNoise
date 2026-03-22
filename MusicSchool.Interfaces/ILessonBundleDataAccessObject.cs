using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleDataAccessObject
    {
        Task<LessonBundle?> GetBundleAsync(int id);
        Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId);
        Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx);
        Task<bool> UpdateAsync(LessonBundle bundle);
    }
}
