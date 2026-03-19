namespace MusicSchool.Data.Interfaces
{
    public interface ILessonAggregateService
    {
        Task<LessonDetail?> GetLessonByIdAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetLessonsByTeacherAndDateAsync(int teacherId, DateOnly scheduledDate);
    }
}
