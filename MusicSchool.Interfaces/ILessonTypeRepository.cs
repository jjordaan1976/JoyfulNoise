using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonTypeRepository
    {
        Task<LessonType?> GetLessonTypeAsync(int id);
        Task<IEnumerable<LessonType>> GetAllActiveAsync();
        Task<int?> AddLessonTypeAsync(LessonType lessonType);
        Task<bool> UpdateLessonTypeAsync(LessonType lessonType);
    }
}
