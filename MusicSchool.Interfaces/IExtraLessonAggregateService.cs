namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonAggregateService
    {
        Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId);
        Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(int teacherId, DateOnly scheduledDate);
    }
}
