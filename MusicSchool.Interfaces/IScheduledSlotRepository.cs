using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotRepository
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);
        Task<int?> AddSlotAsync(ScheduledSlot slot);
        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);
    }
}
