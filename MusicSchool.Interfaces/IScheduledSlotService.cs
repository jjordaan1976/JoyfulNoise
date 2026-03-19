using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotService
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);
        Task<int> InsertAsync(ScheduledSlot slot);
        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);
    }
}
