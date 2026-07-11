using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class LessonTypeDataAccessObject : ILessonTypeDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE LessonTypeID = @LessonTypeID;";

        public static readonly string GetAllActiveSql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE IsActive = 1
                ORDER BY DurationMinutes;";

        public static readonly string InsertSql = @"
                INSERT INTO LessonType (DurationMinutes, BasePricePerLesson, IsActive)
                VALUES (@DurationMinutes, @BasePricePerLesson, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateSql = @"
                UPDATE LessonType
                SET DurationMinutes    = @DurationMinutes,
                    BasePricePerLesson = @BasePricePerLesson,
                    IsActive           = @IsActive
                WHERE LessonTypeID = @LessonTypeID;";

        public LessonTypeDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<LessonType>(GetByIdSql, new { LessonTypeID = id });
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            return await _connection.QueryAsync<LessonType>(GetAllActiveSql);
        }

        public async Task<int> InsertAsync(LessonType lessonType)
        {
            return await _connection.ExecuteScalarAsync<int>(InsertSql, lessonType);
        }

        public async Task<bool> UpdateAsync(LessonType lessonType)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateSql, lessonType);
            return rowsAffected > 0;
        }
    }
}
