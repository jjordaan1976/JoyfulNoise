using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class BundleQuarterDataAccessObject : IBundleQuarterDataAccessObject
    {
        private readonly IDbConnection _connection;

        public BundleQuarterDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT QuarterID,
                       BundleID,
                       QuarterNumber,
                       LessonsAllocated,
                       LessonsUsed,
                       QuarterStartDate,
                       QuarterEndDate
                FROM BundleQuarter
                WHERE BundleID = @BundleID
                ORDER BY QuarterNumber;";

            return await _connection.QueryAsync<BundleQuarter>(sql, new { BundleID = bundleId });
        }

        /// <summary>
        /// Inserts a batch of quarters within an existing transaction.
        /// The connection is passed explicitly so the INSERT runs on the same
        /// connection that owns the transaction — avoiding cross-connection issues
        /// that would cause LessonsAllocated to be 0 or the INSERT to fail silently.
        /// </summary>
        public async Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO BundleQuarter
                    (BundleID, QuarterNumber, LessonsAllocated, LessonsUsed,
                     QuarterStartDate, QuarterEndDate)
                VALUES
                    (@BundleID, @QuarterNumber, @LessonsAllocated, @LessonsUsed,
                     @QuarterStartDate, @QuarterEndDate);";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, quarters, tx));
        }

        public async Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed)
        {
            const string sql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = @LessonsUsed
                WHERE QuarterID = @QuarterID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { QuarterID = quarterId, LessonsUsed = lessonsUsed });
            return rowsAffected > 0;
        }

        /// <summary>
        /// Atomically adjusts LessonsUsed for the quarter that owns the given lesson.
        /// Pass +1 when a lesson is completed or forfeited, -1 when that is reversed.
        /// Clamps to zero so LessonsUsed never goes negative.
        /// </summary>
        public async Task<bool> AdjustLessonsUsedAsync(int lessonId, int delta)
        {
            const string sql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = CASE
                                      WHEN LessonsUsed + @Delta < 0 THEN 0
                                      ELSE LessonsUsed + @Delta
                                  END
                WHERE QuarterID = (
                    SELECT QuarterID FROM Lesson WHERE LessonID = @LessonID
                );";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { LessonID = lessonId, Delta = delta });
            return rowsAffected > 0;
        }
    }
}
