using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Data.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IStudentDataAccessObject _studentService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(
            IStudentDataAccessObject studentService,
            IUserRepository userRepository,
            ILogger<StudentRepository> logger)
        {
            _studentService = studentService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            return await _studentService.GetStudentAsync(id);
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _studentService.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _studentService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            var studentId = await _studentService.InsertAsync(student);

            await _userRepository.CreateUserAsync(new User
            {
                Email = student.Email,
                DisplayName = student.FullName,
                Role = UserRole.Student,
                StudentID = studentId
            });

            return studentId;
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            return await _studentService.UpdateAsync(student);
        }
    }
}
