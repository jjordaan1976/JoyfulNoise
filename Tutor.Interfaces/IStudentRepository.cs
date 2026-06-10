using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetStudentAsync(int id);
        Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId);
        Task<int?> AddStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(Student student);
    }
}
