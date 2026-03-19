using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotRepository
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);

        /// <summary>
        /// Inserts the slot and generates all future Lesson rows for the student's
        /// active bundle. Returns null if the student has no active bundle with
        /// remaining credits, or if the insert fails.
        /// </summary>
        Task<int?> AddSlotAsync(ScheduledSlot slot);

        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);
    }
}
