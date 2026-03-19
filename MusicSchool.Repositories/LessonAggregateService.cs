using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonAggregateService : ILessonAggregateService
    {
        private readonly IDbConnection _connection;

        public static readonly string SELECT_LESSON_DETAIL_QRY = @"
            SELECT l.LessonID,
                   l.SlotID,
                   l.BundleID,
                   l.QuarterID,
                   l.ScheduledDate,
                   l.ScheduledTime,
                   l.Status,
                   l.CreditForfeited,
                   l.CancelledBy,
                   l.CancellationReason,
                   l.OriginalLessonID,
                   l.CompletedAt,
                   s.StudentID,
                   s.FirstName      AS StudentFirstName,
                   s.LastName       AS StudentLastName,
                   t.TeacherID,
                   t.Name           AS TeacherName,
                   lt.LessonTypeID,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM Lesson         l
            JOIN ScheduledSlot  ss ON ss.SlotID       = l.SlotID
            JOIN Student         s ON s.StudentID     = ss.StudentID
            JOIN Teacher         t ON t.TeacherID     = ss.TeacherID
            JOIN LessonType     lt ON lt.LessonTypeID = ss.LessonTypeID
            WHERE l.LessonID = @LessonID;";

        public static readonly string SELECT_LESSONS_BY_TEACHER_DATE_QRY = @"
            SELECT l.LessonID,
                   l.SlotID,
                   l.BundleID,
                   l.QuarterID,
                   l.ScheduledDate,
                   l.ScheduledTime,
                   l.Status,
                   l.CreditForfeited,
                   l.CancelledBy,
                   l.CancellationReason,
                   l.OriginalLessonID,
                   l.CompletedAt,
                   s.StudentID,
                   s.FirstName      AS StudentFirstName,
                   s.LastName       AS StudentLastName,
                   t.TeacherID,
                   t.Name           AS TeacherName,
                   lt.LessonTypeID,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM Lesson         l
            JOIN ScheduledSlot  ss ON ss.SlotID       = l.SlotID
            JOIN Student         s ON s.StudentID     = ss.StudentID
            JOIN Teacher         t ON t.TeacherID     = ss.TeacherID
            JOIN LessonType     lt ON lt.LessonTypeID = ss.LessonTypeID
            WHERE t.TeacherID      = @TeacherID
              AND l.ScheduledDate  = @ScheduledDate
            ORDER BY l.ScheduledTime;";

        public LessonAggregateService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonDetail?> GetLessonByIdAsync(int lessonId)
        {
            return await _connection.QuerySingleOrDefaultAsync<LessonDetail>(
                SELECT_LESSON_DETAIL_QRY,
                new { LessonID = lessonId });
        }

        public async Task<IEnumerable<LessonDetail>> GetLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _connection.QueryAsync<LessonDetail>(
                SELECT_LESSONS_BY_TEACHER_DATE_QRY,
                new { TeacherID = teacherId, ScheduledDate = scheduledDate });
        }
    }
}
