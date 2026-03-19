using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonService
    {
        Task<Lesson?> GetLessonAsync(int id);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Lesson>> GetByStatusAsync(string status);
        Task<int> InsertAsync(Lesson lesson);
        Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt);
    }
}
