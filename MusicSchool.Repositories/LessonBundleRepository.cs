using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleRepository : ILessonBundleRepository
    {
        private readonly ILessonBundleAggregateDataAccessObject _aggregateService;
        private readonly ILessonBundleDataAccessObject _lessonBundleService;
        private readonly ILogger<LessonBundleRepository> _logger;

        public LessonBundleRepository(
            ILessonBundleAggregateDataAccessObject aggregateService,
            ILessonBundleDataAccessObject lessonBundleService,
            ILogger<LessonBundleRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonBundleService = lessonBundleService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the bundle with all four quarters as flat detail rows.
        /// </summary>
        public async Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId)
        {
            return await _aggregateService.GetBundleByIdAsync(bundleId);
        }

        public async Task<IEnumerable<LessonBundleDetail>> GetByStudentAsync(int studentId)
        {
            return await _aggregateService.GetBundleByStudentIdAsync(studentId);
        }

        /// <summary>
        /// Saves the bundle and its 4 quarters atomically.
        /// The application layer is responsible for building the quarter list
        /// before calling this method.
        /// </summary>
        public async Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            try
            {
                return await _aggregateService.SaveNewBundleAsync(bundle, quarters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to save LessonBundle for StudentID {StudentID}",
                    bundle.StudentID);
                return null;
            }
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            try
            {
                return await _lessonBundleService.UpdateAsync(bundle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update BundleID {BundleID}", bundle.BundleID);
                return false;
            }
        }
    }
}
