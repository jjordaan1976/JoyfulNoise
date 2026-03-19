using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(IStudentService studentService, ILogger<StudentRepository> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            return await _studentService.GetStudentAsync(id);
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _studentService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            try
            {
                return await _studentService.InsertAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert Student {FirstName} {LastName}",
                    student.FirstName, student.LastName);
                return null;
            }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                return await _studentService.UpdateAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update StudentID {StudentID}", student.StudentID);
                return false;
            }
        }
    }
}
