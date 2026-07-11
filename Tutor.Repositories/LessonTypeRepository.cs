using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Data.Implementations
{
    public class LessonTypeRepository : ILessonTypeRepository
    {
        private readonly ILessonTypeDataAccessObject _lessonTypeService;
        private readonly ILogger<LessonTypeRepository> _logger;

        public LessonTypeRepository(ILessonTypeDataAccessObject lessonTypeService, ILogger<LessonTypeRepository> logger)
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
            return await _lessonTypeService.InsertAsync(lessonType);
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            return await _lessonTypeService.UpdateAsync(lessonType);
        }
    }
}
