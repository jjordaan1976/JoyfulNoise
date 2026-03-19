using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int> InsertAsync(Teacher teacher);
        Task<bool> UpdateAsync(Teacher teacher);
    }
}
