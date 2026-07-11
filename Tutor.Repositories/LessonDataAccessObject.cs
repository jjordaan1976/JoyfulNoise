using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class LessonDataAccessObject : ILessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
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

        public static readonly string GetByBundleSql = @"
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

        public static readonly string GetByStatusSql = @"
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

        public static readonly string InsertSql = @"
                INSERT INTO Lesson
                    (SlotID, BundleID, QuarterID, ScheduledDate, ScheduledTime,
                     Status, CreditForfeited, CancelledBy, CancellationReason,
                     OriginalLessonID, CompletedAt, Notes)
                VALUES
                    (@SlotID, @BundleID, @QuarterID, @ScheduledDate, @ScheduledTime,
                     @Status, @CreditForfeited, @CancelledBy, @CancellationReason,
                     @OriginalLessonID, @CompletedAt, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateStatusSql = @"
                UPDATE Lesson
                SET Status             = @Status,
                    CreditForfeited    = @CreditForfeited,
                    CancelledBy        = @CancelledBy,
                    CancellationReason = @CancellationReason,
                    CompletedAt        = @CompletedAt,
                    Notes              = COALESCE(@Notes, Notes)
                WHERE LessonID = @LessonID;";

        public static readonly string RescheduleSql = @"
                UPDATE Lesson
                SET ScheduledDate      = @ScheduledDate,
                    ScheduledTime      = @ScheduledTime,
                    Status             = @Status,
                    CreditForfeited    = 0,
                    CancelledBy        = NULL,
                    CancellationReason = NULL,
                    CompletedAt        = NULL
                WHERE LessonID = @LessonID;";

        public LessonDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Lesson?> GetLessonAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<Lesson>(GetByIdSql, new { LessonID = id });
        }

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
        {
            return await _connection.QueryAsync<Lesson>(GetByBundleSql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Lesson>> GetByStatusAsync(string status)
        {
            return await _connection.QueryAsync<Lesson>(GetByStatusSql, new { Status = status });
        }

        public async Task<int> InsertAsync(Lesson lesson)
            => await InsertAsync(lesson, _connection, null!);

        /// <summary>
        /// Inserts within an existing transaction. Executes against the passed-in
        /// connection (the one that owns the transaction), never the injected one.
        /// </summary>
        public async Task<int> InsertAsync(Lesson lesson, IDbConnection connection, IDbTransaction transaction)
        {
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(InsertSql, lesson, transaction));
        }

        public async Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateStatusSql, new
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
            var rowsAffected = await _connection.ExecuteAsync(RescheduleSql, new
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
