using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class BundleQuarterDataAccessObject : IBundleQuarterDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByBundleSql = @"
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

        public static readonly string InsertBatchSql = @"
                INSERT INTO BundleQuarter
                    (BundleID, QuarterNumber, LessonsAllocated, LessonsUsed,
                     QuarterStartDate, QuarterEndDate)
                VALUES
                    (@BundleID, @QuarterNumber, @LessonsAllocated, @LessonsUsed,
                     @QuarterStartDate, @QuarterEndDate);";

        public static readonly string UpdateLessonsUsedSql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = @LessonsUsed
                WHERE QuarterID = @QuarterID;";

        public static readonly string AdjustLessonsUsedSql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = CASE
                                      WHEN LessonsUsed + @Delta < 0 THEN 0
                                      ELSE LessonsUsed + @Delta
                                  END
                WHERE QuarterID = (
                    SELECT QuarterID FROM Lesson WHERE LessonID = @LessonID
                );";

        public BundleQuarterDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId)
        {
            return await _connection.QueryAsync<BundleQuarter>(GetByBundleSql, new { BundleID = bundleId });
        }

        /// <summary>
        /// Inserts a batch of quarters within an existing transaction.
        /// The connection is passed explicitly so the INSERT runs on the same
        /// connection that owns the transaction — avoiding cross-connection issues
        /// that would cause LessonsAllocated to be 0 or the INSERT to fail silently.
        /// </summary>
        public async Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx, IDbConnection connection)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(InsertBatchSql, quarters, tx));
        }

        public async Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateLessonsUsedSql,
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
            var rowsAffected = await _connection.ExecuteAsync(AdjustLessonsUsedSql,
                new { LessonID = lessonId, Delta = delta });
            return rowsAffected > 0;
        }
    }
}
