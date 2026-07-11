using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Data.Implementations
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
        /// Throws InvalidOperationException if the student has no usable bundle.
        /// </summary>
        public async Task<int?> AddSlotAsync(ScheduledSlot slot)
        {
            return await _aggregateService.SaveNewSlotWithLessonsAsync(slot);
        }

        /// <summary>
        /// Closes a slot by setting EffectiveTo and IsActive = false.
        /// Call AddSlotAsync afterwards to open the replacement slot.
        /// </summary>
        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            return await _slotService.CloseSlotAsync(slotId, effectiveTo);
        }
    }
}
