using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotService
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(ScheduledSlot slot);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(ScheduledSlot slot, IDbTransaction tx);

        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);

        /// <summary>
        /// Opens a connection (if not already open), begins a transaction,
        /// invokes <paramref name="work"/>, and commits. Rolls back on exception.
        /// </summary>
        Task ExecuteInTransactionAsync(Func<IDbTransaction, IDbConnection, Task> work);
    }
}
