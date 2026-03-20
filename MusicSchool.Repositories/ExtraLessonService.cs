using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonService : IExtraLessonService
    {
        private readonly IDbConnection _connection;

        public ExtraLessonService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ExtraLesson?> GetExtraLessonAsync(int id)
        {
            const string sql = @"
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

            return await _connection.QuerySingleOrDefaultAsync<ExtraLesson>(sql, new { ExtraLessonID = id });
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            const string sql = @"
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

            return await _connection.QueryAsync<ExtraLesson>(sql, new { StudentID = studentId });
        }

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson)
            => await InsertAsync(extraLesson, null!, _connection);

        /// <summary>Inserts within an existing transaction.</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO ExtraLesson
                    (StudentID, TeacherID, LessonTypeID, ScheduledDate,
                     ScheduledTime, PriceCharged, Status, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @ScheduledDate,
                     @ScheduledTime, @PriceCharged, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, extraLesson, tx));
        }

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int extraLessonId, string status, string? note = null)
        {
            const string sql = @"
                UPDATE ExtraLesson
                SET Status = @Status,
                    Notes  = COALESCE(@Notes, Notes)
                WHERE ExtraLessonID = @ExtraLessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { ExtraLessonID = extraLessonId, Status = status, Notes = note });
            return rowsAffected > 0;
        }
    }
}
