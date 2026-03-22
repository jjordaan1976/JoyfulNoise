using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotRepository : IScheduledSlotRepository
    {
        private readonly IScheduledSlotAggregateDataAccessObject _aggregateService;
        private readonly IScheduledSlotDataAccessObject _slotService;
        private readonly ILogger<ScheduledSlotRepository> _logger;

        public ScheduledSlotRepository(
            IScheduledSlotAggregateDataAccessObject aggregateService,
            IScheduledSlotDataAccessObject slotService,
            ILogger<ScheduledSlotRepository> logger)
        {
            _aggregateService = aggregateService;
            _slotService = slotService;
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
                return await _aggregateService.SaveNewSlotWithLessonsAsync(slot);
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violation (no active bundle) — warn rather than error.
                _logger.LogWarning(ex.Message);
                return null;
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
    }
}
