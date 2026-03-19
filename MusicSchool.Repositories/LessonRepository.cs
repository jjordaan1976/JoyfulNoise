using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ILessonAggregateService _aggregateService;
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(
            ILessonAggregateService aggregateService,
            ILessonService lessonService,
            ILogger<LessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonService = lessonService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a single lesson with full context (student, teacher, lesson type).
        /// </summary>
        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
        {
            return await _aggregateService.GetLessonByIdAsync(lessonId);
        }

        /// <summary>
        /// Returns all lessons for a teacher on a given date, with full context.
        /// </summary>
        public async Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateOnly scheduledDate)
        {
            return await _aggregateService.GetLessonsByTeacherAndDateAsync(teacherId, scheduledDate);
        }

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
        {
            return await _lessonService.GetByBundleAsync(bundleId);
        }

        public async Task<int?> AddLessonAsync(Lesson lesson)
        {
            try
            {
                return await _lessonService.InsertAsync(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert Lesson for BundleID {BundleID} on {ScheduledDate}",
                    lesson.BundleID, lesson.ScheduledDate);
                return null;
            }
        }

        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status,
            bool creditForfeited, string? cancelledBy, string? cancellationReason,
            DateTime? completedAt)
        {
            try
            {
                return await _lessonService.UpdateStatusAsync(
                    lessonId, status, creditForfeited,
                    cancelledBy, cancellationReason, completedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for LessonID {LessonID}", lessonId);
                return false;
            }
        }
    }
}
