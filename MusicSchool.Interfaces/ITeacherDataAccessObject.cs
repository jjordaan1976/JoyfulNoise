using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ITeacherDataAccessObject
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int> InsertAsync(Teacher teacher);
        Task<bool> UpdateAsync(Teacher teacher);
    }
}
