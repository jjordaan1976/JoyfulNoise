using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface ILessonTypeDataAccessObject
    {
        Task<LessonType?> GetLessonTypeAsync(int id);
        Task<IEnumerable<LessonType>> GetAllActiveAsync();
        Task<int> InsertAsync(LessonType lessonType);
        Task<bool> UpdateAsync(LessonType lessonType);
    }
}
