using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class LessonBundleDataAccessObject : ILessonBundleDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       TotalLessons,
                       PricePerLesson,
                       StartDate,
                       EndDate,
                       QuarterSize,
                       IsActive,
                       Notes,
                       CreatedAt
                FROM LessonBundle
                WHERE BundleID = @BundleID;";

        public static readonly string GetByStudentSql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       TotalLessons,
                       PricePerLesson,
                       StartDate,
                       EndDate,
                       QuarterSize,
                       IsActive,
                       Notes,
                       CreatedAt
                FROM LessonBundle
                WHERE StudentID = @StudentID
ORDER BY StudentID";

        public static readonly string InsertSql = @"
                INSERT INTO LessonBundle
                    (StudentID, TeacherID, LessonTypeID,
                     TotalLessons, PricePerLesson, StartDate, EndDate, IsActive, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID,
                     @TotalLessons, @PricePerLesson, @StartDate, @EndDate, @IsActive, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateSql = @"
                UPDATE LessonBundle
                SET StudentID      = @StudentID,
                    TeacherID      = @TeacherID,
                    LessonTypeID   = @LessonTypeID,
                    AcademicYear   = @AcademicYear,
                    TotalLessons   = @TotalLessons,
                    PricePerLesson = @PricePerLesson,
                    StartDate      = @StartDate,
                    EndDate        = @EndDate,
                    IsActive       = @IsActive,
                    Notes          = @Notes
                WHERE BundleID = @BundleID;";

        public LessonBundleDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonBundle?> GetBundleAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<LessonBundle>(GetByIdSql, new { BundleID = id });
        }

        public async Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId)
        {
            return await _connection.QueryAsync<LessonBundle>(GetByStudentSql, new { StudentID = studentId });
        }

        /// <summary>
        /// Inserts within an existing transaction. Executes against the passed-in
        /// connection (the one that owns the transaction), never the injected one.
        /// </summary>
        public async Task<int> InsertAsync(LessonBundle bundle, IDbConnection connection, IDbTransaction transaction)
        {
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(InsertSql, bundle, transaction));
        }

        public async Task<bool> UpdateAsync(LessonBundle bundle)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateSql, bundle);
            return rowsAffected > 0;
        }
    }
}
