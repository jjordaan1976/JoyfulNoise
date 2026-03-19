using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonService
    {
        Task<ExtraLesson?> GetExtraLessonAsync(int id);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(ExtraLesson extraLesson);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection);

        Task<bool> UpdateStatusAsync(int extraLessonId, string status);
    }
}
