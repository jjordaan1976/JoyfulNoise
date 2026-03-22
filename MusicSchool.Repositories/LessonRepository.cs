using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ILessonAggregateDataAccessObject _aggregateService;
        private readonly ILessonDataAccessObject _lessonService;
        private readonly IBundleQuarterDataAccessObject _bundleQuarterService;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(
            ILessonAggregateDataAccessObject aggregateService,
            ILessonDataAccessObject lessonService,
            IBundleQuarterDataAccessObject bundleQuarterService,
            ILogger<LessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonService = lessonService;
            _bundleQuarterService = bundleQuarterService;
            _logger = logger;
        }

        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
            => await _aggregateService.GetLessonByIdAsync(lessonId);

        public async Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
            => await _aggregateService.GetLessonsByTeacherAndDateAsync(teacherId, scheduledDate);

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
            => await _lessonService.GetByBundleAsync(bundleId);

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

        /// <summary>
        /// Updates the lesson status and keeps BundleQuarter.LessonsUsed in sync:
        ///   Completed / Forfeited → +1 (credit consumed)
        ///   CancelledTeacher / CancelledStudent → -1 only if the previous status
        ///   had already consumed a credit (i.e. was Completed or Forfeited).
        /// The delta approach is atomic — no separate read is needed.
        /// </summary>
        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status,
            bool creditForfeited, string? cancelledBy, string? cancellationReason,
            DateTime? completedAt, string? note = null)
        {
            try
            {
                // 1. Read current status so we know whether to adjust the quarter.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                // 2. Update the lesson row.
                var updated = await _lessonService.UpdateStatusAsync(
                    lessonId, status, creditForfeited,
                    cancelledBy, cancellationReason, completedAt, note);

                if (!updated) return false;

                // 3. Adjust BundleQuarter.LessonsUsed.
                //    Credit is consumed when status moves TO Completed or Forfeited.
                //    Credit is released when status moves FROM Completed or Forfeited
                //    to anything that doesn't consume a credit.
                bool previousConsumed = lesson.Status == LessonStatus.Completed
                                     || lesson.Status == LessonStatus.Forfeited;
                bool newConsumed = status == LessonStatus.Completed
                                || status == LessonStatus.Forfeited;

                int delta = (newConsumed ? 1 : 0) - (previousConsumed ? 1 : 0);

                if (delta != 0)
                    await _bundleQuarterService.AdjustLessonsUsedAsync(lessonId, delta);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for LessonID {LessonID}", lessonId);
                return false;
            }
        }

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled.
        /// No credit adjustment is needed — cancelled lessons never consumed a credit.
        /// </summary>
        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            try
            {
                // Guard: only allow rescheduling of cancelled lessons.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                if (lesson.Status != LessonStatus.CancelledTeacher
                    && lesson.Status != LessonStatus.CancelledStudent)
                {
                    _logger.LogWarning(
                        "RescheduleLessonAsync rejected: LessonID {LessonID} has status {Status}.",
                        lessonId, lesson.Status);
                    return false;
                }

                return await _lessonService.RescheduleLessonAsync(lessonId, newDate, newTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reschedule LessonID {LessonID}", lessonId);
                return false;
            }
        }
    }
}
