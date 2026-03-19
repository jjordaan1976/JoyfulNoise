using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotRepository : IScheduledSlotRepository
    {
        private readonly IScheduledSlotService _slotService;
        private readonly ILogger<ScheduledSlotRepository> _logger;

        public ScheduledSlotRepository(IScheduledSlotService slotService, ILogger<ScheduledSlotRepository> logger)
        {
            _slotService = slotService;
            _logger = logger;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            return await _slotService.GetSlotAsync(id);
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            return await _slotService.GetActiveByStudentAsync(studentId);
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            return await _slotService.GetActiveByTeacherAsync(teacherId);
        }

        public async Task<int?> AddSlotAsync(ScheduledSlot slot)
        {
            try
            {
                return await _slotService.InsertAsync(slot);
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
