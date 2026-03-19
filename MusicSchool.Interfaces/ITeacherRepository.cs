using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int?> AddTeacherAsync(Teacher teacher);
        Task<bool> UpdateTeacherAsync(Teacher teacher);
    }
}
