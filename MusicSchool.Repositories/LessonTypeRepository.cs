using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeRepository : ILessonTypeRepository
    {
        private readonly ILessonTypeService _lessonTypeService;
        private readonly ILogger<LessonTypeRepository> _logger;

        public LessonTypeRepository(ILessonTypeService lessonTypeService, ILogger<LessonTypeRepository> logger)
        {
            _lessonTypeService = lessonTypeService;
            _logger = logger;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            return await _lessonTypeService.GetLessonTypeAsync(id);
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            return await _lessonTypeService.GetAllActiveAsync();
        }

        public async Task<int?> AddLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                return await _lessonTypeService.InsertAsync(lessonType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert LessonType {DurationMinutes}min",
                    lessonType.DurationMinutes);
                return null;
            }
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                return await _lessonTypeService.UpdateAsync(lessonType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update LessonTypeID {LessonTypeID}",
                    lessonType.LessonTypeID);
                return false;
            }
        }
    }
}
