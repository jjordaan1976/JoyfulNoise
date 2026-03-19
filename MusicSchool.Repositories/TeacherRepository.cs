using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(ITeacherService teacherService, ILogger<TeacherRepository> logger)
        {
            _teacherService = teacherService;
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
            try
            {
                return await _teacherService.InsertAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert Teacher {Name}", teacher.Name);
                return null;
            }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                return await _teacherService.UpdateAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update TeacherID {TeacherID}", teacher.TeacherID);
                return false;
            }
        }
    }
}
