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
        /// Builds the quarter layout for the requested creation mode and saves the
        /// bundle, its quarters, and invoice instalments atomically.
        ///
        /// Full:    all four calendar quarters of the start year, each at TotalLessons/4.
        ///          Only valid for January/February starts.
        /// Prorata: only the calendar quarters remaining from the start date, each at
        ///          the bundle's normal per-quarter rate; TotalLessons is reduced to match
        ///          (e.g. a 32-lesson bundle starting in July becomes 2 quarters × 8 = 16).
        /// </summary>
        public async Task<int?> AddBundleAsync(LessonBundle bundle, BundleCreationMode mode)
        {
            if (bundle.TotalLessons <= 0 || bundle.TotalLessons % 4 != 0)
                throw new InvalidOperationException("Total lessons must be a positive number divisible by 4.");

            if (mode == BundleCreationMode.Full && bundle.StartDate.Month > 2)
                throw new InvalidOperationException("A full-year bundle can only start in January or February.");

            var perQuarter   = bundle.TotalLessons / 4;
            var year         = bundle.StartDate.Year;
            var firstQuarter = mode == BundleCreationMode.Full
                ? 1
                : (bundle.StartDate.Month - 1) / 3 + 1;

            var quarters = new List<BundleQuarter>();
            for (var q = firstQuarter; q <= 4; q++)
            {
                var quarterStart = new DateTime(year, (q - 1) * 3 + 1, 1);
                quarters.Add(new BundleQuarter
                {
                    QuarterNumber    = (byte)q,
                    LessonsAllocated = perQuarter,
                    QuarterStartDate = q == firstQuarter && bundle.StartDate > quarterStart
                        ? bundle.StartDate
                        : quarterStart,
                    QuarterEndDate   = quarterStart.AddMonths(3).AddDays(-1)
                });
            }

            bundle.TotalLessons = perQuarter * quarters.Count;
            bundle.EndDate      = new DateTime(year, 12, 31);

            return await _aggregateService.SaveNewBundleAsync(bundle, quarters);
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            return await _lessonBundleService.UpdateAsync(bundle);
        }
    }
}
