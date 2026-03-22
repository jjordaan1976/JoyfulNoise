using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonDataAccessObject : ILessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Lesson?> GetLessonAsync(int id)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE LessonID = @LessonID;";

            return await _connection.QuerySingleOrDefaultAsync<Lesson>(sql, new { LessonID = id });
        }

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE BundleID = @BundleID
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Lesson>> GetByStatusAsync(string status)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE Status = @Status
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { Status = status });
        }

        public async Task<int> InsertAsync(Lesson lesson)
            => await InsertAsync(lesson, null!);

        public async Task<int> InsertAsync(Lesson lesson, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO Lesson
                    (SlotID, BundleID, QuarterID, ScheduledDate, ScheduledTime,
                     Status, CreditForfeited, CancelledBy, CancellationReason,
                     OriginalLessonID, CompletedAt, Notes)
                VALUES
                    (@SlotID, @BundleID, @QuarterID, @ScheduledDate, @ScheduledTime,
                     @Status, @CreditForfeited, @CancelledBy, @CancellationReason,
                     @OriginalLessonID, @CompletedAt, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, lesson, tx));
        }

        public async Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null)
        {
            const string sql = @"
                UPDATE Lesson
                SET Status             = @Status,
                    CreditForfeited    = @CreditForfeited,
                    CancelledBy        = @CancelledBy,
                    CancellationReason = @CancellationReason,
                    CompletedAt        = @CompletedAt,
                    Notes              = COALESCE(@Notes, Notes)
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID           = lessonId,
                Status             = status,
                CreditForfeited    = creditForfeited,
                CancelledBy        = cancelledBy,
                CancellationReason = cancellationReason,
                CompletedAt        = completedAt,
                Notes              = note
            });
            return rowsAffected > 0;
        }

        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            const string sql = @"
                UPDATE Lesson
                SET ScheduledDate      = @ScheduledDate,
                    ScheduledTime      = @ScheduledTime,
                    Status             = @Status,
                    CreditForfeited    = 0,
                    CancelledBy        = NULL,
                    CancellationReason = NULL,
                    CompletedAt        = NULL
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID      = lessonId,
                ScheduledDate = newDate,
                ScheduledTime = newTime,
                Status        = LessonStatus.Scheduled
            });
            return rowsAffected > 0;
        }
    }
}
