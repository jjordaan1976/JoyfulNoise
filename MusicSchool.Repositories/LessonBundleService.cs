using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleService : ILessonBundleService
    {
        private readonly IDbConnection _connection;

        public LessonBundleService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonBundle?> GetBundleAsync(int id)
        {
            const string sql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       AcademicYear,
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

            return await _connection.QuerySingleOrDefaultAsync<LessonBundle>(sql, new { BundleID = id });
        }

        public async Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       AcademicYear,
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
                ORDER BY AcademicYear DESC;";

            return await _connection.QueryAsync<LessonBundle>(sql, new { StudentID = studentId });
        }

        public async Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO LessonBundle
                    (StudentID, TeacherID, LessonTypeID, AcademicYear,
                     TotalLessons, PricePerLesson, StartDate, EndDate, IsActive, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @AcademicYear,
                     @TotalLessons, @PricePerLesson, @StartDate, @EndDate, @IsActive, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, bundle, tx));
        }

        public async Task<bool> UpdateAsync(LessonBundle bundle)
        {
            const string sql = @"
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

            var rowsAffected = await _connection.ExecuteAsync(sql, bundle);
            return rowsAffected > 0;
        }
    }
}
