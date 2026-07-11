using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Data.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ITeacherDataAccessObject _teacherService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(
            ITeacherDataAccessObject teacherService,
            IUserRepository userRepository,
            ILogger<TeacherRepository> logger)
        {
            _teacherService = teacherService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            return await _teacherService.GetTeacherAsync(id);
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            return await _teacherService.GetAllActiveAsync();
        }

        public async Task<int?> AddTeacherAsync(Teacher teacher)
        {
            var teacherId = await _teacherService.InsertAsync(teacher);

            await _userRepository.CreateUserAsync(new User
            {
                Email = teacher.Email,
                DisplayName = teacher.Name,
                Role = UserRole.Teacher,
                TeacherID = teacherId
            });

            return teacherId;
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            return await _teacherService.UpdateAsync(teacher);
        }
    }
}
