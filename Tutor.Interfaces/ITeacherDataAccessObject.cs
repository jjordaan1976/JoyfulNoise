using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface ITeacherDataAccessObject
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int> InsertAsync(Teacher teacher);
        Task<bool> UpdateAsync(Teacher teacher);
    }
}
