using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotAggregateDataAccessObject
    {
        /// <summary>
        /// Finds the student's active bundle with remaining credits, inserts the slot,
        /// and generates all future Lesson rows atomically in a single transaction.
        /// Returns the new SlotID.
        /// Throws <see cref="InvalidOperationException"/> when no usable bundle exists.
        /// </summary>
        Task<int> SaveNewSlotWithLessonsAsync(ScheduledSlot slot);
    }
}
