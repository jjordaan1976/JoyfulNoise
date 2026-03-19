using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IStudentService
    {
        Task<Student?> GetStudentAsync(int id);
        Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId);
        Task<int> InsertAsync(Student student);
        Task<bool> UpdateAsync(Student student);
    }
}
