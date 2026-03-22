using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonTypeDataAccessObject
    {
        Task<LessonType?> GetLessonTypeAsync(int id);
        Task<IEnumerable<LessonType>> GetAllActiveAsync();
        Task<int> InsertAsync(LessonType lessonType);
        Task<bool> UpdateAsync(LessonType lessonType);
    }
}
