using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models;

namespace Tutor.Data.Implementations
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
        public async Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters, int selectedBundleLessons)
        {
            return await _aggregateService.SaveNewBundleAsync(bundle, quarters, selectedBundleLessons);
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            return await _lessonBundleService.UpdateAsync(bundle);
        }
    }
}
