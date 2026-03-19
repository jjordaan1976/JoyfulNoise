using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonAggregateService : IExtraLessonAggregateService
    {
        private readonly IDbConnection _connection;

        public static readonly string SELECT_EXTRA_LESSON_DETAIL_QRY = @"
            SELECT el.ExtraLessonID,
                   el.StudentID,
                   el.TeacherID,
                   el.LessonTypeID,
                   el.ScheduledDate,
                   el.ScheduledTime,
                   el.PriceCharged,
                   el.Status,
                   el.Notes,
                   s.FirstName       AS StudentFirstName,
                   s.LastName        AS StudentLastName,
                   t.Name            AS TeacherName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM ExtraLesson  el
            JOIN Student       s ON s.StudentID     = el.StudentID
            JOIN Teacher       t ON t.TeacherID     = el.TeacherID
            JOIN LessonType   lt ON lt.LessonTypeID = el.LessonTypeID
            WHERE el.ExtraLessonID = @ExtraLessonID;";

        public static readonly string SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY = @"
            SELECT el.ExtraLessonID,
                   el.StudentID,
                   el.TeacherID,
                   el.LessonTypeID,
                   el.ScheduledDate,
                   el.ScheduledTime,
                   el.PriceCharged,
                   el.Status,
                   el.Notes,
                   s.FirstName       AS StudentFirstName,
                   s.LastName        AS StudentLastName,
                   t.Name            AS TeacherName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM ExtraLesson  el
            JOIN Student       s ON s.StudentID     = el.StudentID
            JOIN Teacher       t ON t.TeacherID     = el.TeacherID
            JOIN LessonType   lt ON lt.LessonTypeID = el.LessonTypeID
            WHERE el.TeacherID    = @TeacherID
              AND el.ScheduledDate = @ScheduledDate
            ORDER BY el.ScheduledTime;";

        public ExtraLessonAggregateService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId)
        {
            return await _connection.QuerySingleOrDefaultAsync<ExtraLessonDetail>(
                SELECT_EXTRA_LESSON_DETAIL_QRY,
                new { ExtraLessonID = extraLessonId });
        }

        public async Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _connection.QueryAsync<ExtraLessonDetail>(
                SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY,
                new { TeacherID = teacherId, ScheduledDate = scheduledDate });
        }
    }
}
