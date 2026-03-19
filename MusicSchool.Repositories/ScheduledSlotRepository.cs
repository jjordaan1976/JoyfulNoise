using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotRepository : IScheduledSlotRepository
    {
        private readonly IScheduledSlotService _slotService;
        private readonly ILessonBundleService _bundleService;
        private readonly IBundleQuarterService _quarterService;
        private readonly ILessonService _lessonService;
        private readonly ILogger<ScheduledSlotRepository> _logger;

        public ScheduledSlotRepository(
            IScheduledSlotService slotService,
            ILessonBundleService bundleService,
            IBundleQuarterService quarterService,
            ILessonService lessonService,
            ILogger<ScheduledSlotRepository> logger)
        {
            _slotService = slotService;
            _bundleService = bundleService;
            _quarterService = quarterService;
            _lessonService = lessonService;
            _logger = logger;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
            => await _slotService.GetSlotAsync(id);

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
            => await _slotService.GetActiveByStudentAsync(studentId);

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
            => await _slotService.GetActiveByTeacherAsync(teacherId);

        /// <summary>
        /// Validates that the student has an active bundle with remaining credits,
        /// inserts the slot, then generates all future Lesson rows up to the bundle's
        /// EndDate — one per weekly occurrence matching the slot's DayOfWeek.
        /// Everything runs in a single transaction; nothing is committed if any step fails.
        /// Returns null if the student has no usable bundle, or on any error.
        /// </summary>
        public async Task<int?> AddSlotAsync(ScheduledSlot slot)
        {
            try
            {
                // 1. Find the student's active bundle that still has remaining credits.
                //    "Active" means IsActive = true, not yet expired, and at least one
                //    quarter still has lessons remaining.
                var bundles = await _bundleService.GetByStudentAsync(slot.StudentID);

                var today = DateTime.Today;
                LessonBundle? bundle = null;

                foreach (var b in bundles.Where(b => b.IsActive && b.EndDate >= today))
                {
                    var quartersl = (await _quarterService.GetByBundleAsync(b.BundleID)).ToList();
                    if (quartersl.Any(q => q.LessonsUsed < q.LessonsAllocated))
                    {
                        bundle = b;
                        break;
                    }
                }

                if (bundle is null)
                {
                    _logger.LogWarning(
                        "AddSlotAsync rejected: StudentID {StudentID} has no active bundle with remaining credits.",
                        slot.StudentID);
                    return null;
                }

                var quarters = (await _quarterService.GetByBundleAsync(bundle.BundleID)).ToList();

                // 2. Insert the slot and generate lessons in one transaction.
                await _slotService.ExecuteInTransactionAsync(async (tx, conn) =>
                {
                    var slotId = await _slotService.InsertAsync(slot, tx);
                    slot.SlotID = slotId ;

                    // Generate one Lesson per weekly occurrence from EffectiveFrom
                    // (or today, whichever is later) through the bundle's EndDate.
                    var generateFrom = slot.EffectiveFrom.Date > today
                        ? slot.EffectiveFrom.Date
                        : today;

                    var lessonDates = GetOccurrences(generateFrom, bundle.EndDate, slot.DayOfWeek);

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
                });

                return slot.SlotID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert ScheduledSlot for StudentID {StudentID}", slot.StudentID);
                return null;
            }
        }

        /// <summary>
        /// Closes a slot by setting EffectiveTo and IsActive = false.
        /// Call AddSlotAsync afterwards to open the replacement slot.
        /// </summary>
        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            try
            {
                return await _slotService.CloseSlotAsync(slotId, effectiveTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close SlotID {SlotID}", slotId);
                return false;
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
