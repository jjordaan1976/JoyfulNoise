namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonAggregateDataAccessObject
    {
        Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId);
        Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
    }
}
