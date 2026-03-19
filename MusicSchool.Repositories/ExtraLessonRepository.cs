using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonRepository : IExtraLessonRepository
    {
        private readonly IExtraLessonAggregateService _aggregateService;
        private readonly IExtraLessonService _extraLessonService;
        private readonly ILogger<ExtraLessonRepository> _logger;

        public ExtraLessonRepository(
            IExtraLessonAggregateService aggregateService,
            IExtraLessonService extraLessonService,
            ILogger<ExtraLessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _extraLessonService = extraLessonService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a single extra lesson with full context (student, teacher, lesson type).
        /// </summary>
        public async Task<ExtraLessonDetail?> GetExtraLessonAsync(int extraLessonId)
        {
            return await _aggregateService.GetExtraLessonByIdAsync(extraLessonId);
        }

        /// <summary>
        /// Returns all extra lessons for a teacher on a given date, with full context.
        /// </summary>
        public async Task<IEnumerable<ExtraLessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _aggregateService.GetExtraLessonsByTeacherAndDateAsync(teacherId, scheduledDate);
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            return await _extraLessonService.GetByStudentAsync(studentId);
        }

        public async Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson)
        {
            try
            {
                return await _extraLessonService.InsertAsync(extraLesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert ExtraLesson for StudentID {StudentID} on {ScheduledDate}",
                    extraLesson.StudentID, extraLesson.ScheduledDate);
                return null;
            }
        }

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status)
        {
            try
            {
                return await _extraLessonService.UpdateStatusAsync(extraLessonId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to update status for ExtraLessonID {ExtraLessonID}", extraLessonId);
                return false;
            }
        }
    }
}
