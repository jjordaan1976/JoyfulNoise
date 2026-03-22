using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotAggregateDataAccessObject : IScheduledSlotAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;
        private readonly IScheduledSlotDataAccessObject _slotService;
        private readonly ILessonBundleDataAccessObject _bundleService;
        private readonly IBundleQuarterDataAccessObject _quarterService;
        private readonly ILessonDataAccessObject _lessonService;

        public ScheduledSlotAggregateDataAccessObject(
            IDbConnection connection,
            IScheduledSlotDataAccessObject slotService,
            ILessonBundleDataAccessObject bundleService,
            IBundleQuarterDataAccessObject quarterService,
            ILessonDataAccessObject lessonService)
        {
            _connection = connection;
            _slotService = slotService;
            _bundleService = bundleService;
            _quarterService = quarterService;
            _lessonService = lessonService;
        }

        /// <summary>
        /// Finds the student's active bundle with remaining credits, inserts the slot,
        /// then generates all future Lesson rows up to the bundle's EndDate — one per
        /// weekly occurrence matching the slot's DayOfWeek — all in a single transaction.
        /// Returns the new SlotID, or throws if the student has no usable bundle.
        /// </summary>
        public async Task<int> SaveNewSlotWithLessonsAsync(ScheduledSlot slot)
        {
            // 1. Find the student's active bundle that still has remaining credits.
            //    "Active" means IsActive = true, not yet expired, and at least one
            //    quarter still has lessons remaining.
            var bundles = await _bundleService.GetByStudentAsync(slot.StudentID);

            LessonBundle? bundle = null;

            foreach (var b in bundles.Where(b => b.IsActive && b.EndDate >= DateTime.Today))
            {
                var bundleQuarters = (await _quarterService.GetByBundleAsync(b.BundleID)).ToList();
                if (bundleQuarters.Any(q => q.LessonsUsed < q.LessonsAllocated))
                {
                    bundle = b;
                    break;
                }
            }

            if (bundle is null)
                throw new InvalidOperationException(
                    $"StudentID {slot.StudentID} has no active bundle with remaining credits.");

            var quarters = (await _quarterService.GetByBundleAsync(bundle.BundleID)).ToList();

            // 2. Insert the slot and generate lessons in one transaction.
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var tx = _connection.BeginTransaction();
            try
            {
                var slotId = await _slotService.InsertAsync(slot, _connection, tx);
                slot.SlotID = slotId;

                // Generate one Lesson per weekly occurrence from the slot's EffectiveFrom
                // through the bundle's EndDate. EffectiveFrom is authoritative — do not
                // clamp to today, as slots may be created retroactively or in advance.
                var lessonDates = GetOccurrences(slot.EffectiveFrom.Date, bundle.EndDate, slot.DayOfWeek);

                foreach (var date in lessonDates)
                {
                    var quarter = quarters.FirstOrDefault(q =>
                        date >= q.QuarterStartDate && date <= q.QuarterEndDate);

                    if (quarter is null) continue;

                    await _lessonService.InsertAsync(new Lesson
                    {
                        SlotID          = slotId,
                        BundleID        = bundle.BundleID,
                        QuarterID       = quarter.QuarterID,
                        ScheduledDate   = date,
                        ScheduledTime   = slot.SlotTime,
                        Status          = LessonStatus.Scheduled,
                        CreditForfeited = false,
                    }, tx);
                }

                tx.Commit();
                return slotId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Enumerates every calendar date between <paramref name="from"/> and
        /// <paramref name="to"/> (inclusive) that falls on <paramref name="isoDayOfWeek"/>
        /// (1 = Monday … 7 = Sunday, matching the ScheduledSlot.DayOfWeek convention).
        /// </summary>
        private static IEnumerable<DateTime> GetOccurrences(
            DateTime from, DateTime to, byte isoDayOfWeek)
        {
            // .NET DayOfWeek: Sunday=0, Monday=1 … Saturday=6
            // ISO DayOfWeek:  Monday=1, Tuesday=2 … Sunday=7
            var targetDotNet = isoDayOfWeek == 7
                ? DayOfWeek.Sunday
                : (DayOfWeek)isoDayOfWeek;

            var date = from;
            // Advance to the first matching weekday
            while (date.DayOfWeek != targetDotNet)
                date = date.AddDays(1);

            while (date <= to)
            {
                yield return date;
                date = date.AddDays(7);
            }
        }
    }
}
