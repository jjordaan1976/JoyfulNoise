using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class ExtraLessonDataAccessObject : IExtraLessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT ExtraLessonID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       ScheduledDate,
                       ScheduledTime,
                       PriceCharged,
                       Status,
                       Notes,
                       CreatedAt
                FROM ExtraLesson
                WHERE ExtraLessonID = @ExtraLessonID;";

        public static readonly string GetByStudentSql = @"
                SELECT ExtraLessonID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       ScheduledDate,
                       ScheduledTime,
                       PriceCharged,
                       Status,
                       Notes,
                       CreatedAt
                FROM ExtraLesson
                WHERE StudentID = @StudentID
                ORDER BY ScheduledDate DESC, ScheduledTime DESC;";

        public static readonly string InsertSql = @"
                INSERT INTO ExtraLesson
                    (StudentID, TeacherID, LessonTypeID, ScheduledDate,
                     ScheduledTime, PriceCharged, Status, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @ScheduledDate,
                     @ScheduledTime, @PriceCharged, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateStatusSql = @"
                UPDATE ExtraLesson
                SET Status = @Status,
                    Notes  = COALESCE(@Notes, Notes)
                WHERE ExtraLessonID = @ExtraLessonID;";

        public ExtraLessonDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ExtraLesson?> GetExtraLessonAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<ExtraLesson>(GetByIdSql, new { ExtraLessonID = id });
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            return await _connection.QueryAsync<ExtraLesson>(GetByStudentSql, new { StudentID = studentId });
        }

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson)
            => await InsertAsync(extraLesson, null!, _connection);

        /// <summary>
        /// Inserts within an existing transaction. Executes against the passed-in
        /// connection (the one that owns the transaction), never the injected one.
        /// </summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection)
        {
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(InsertSql, extraLesson, tx));
        }

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int extraLessonId, string status, string? note = null)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateStatusSql,
                new { ExtraLessonID = extraLessonId, Status = status, Notes = note });
            return rowsAffected > 0;
        }
    }
}
