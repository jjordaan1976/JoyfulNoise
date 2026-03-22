using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeDataAccessObject : ILessonTypeDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonTypeDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            const string sql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE LessonTypeID = @LessonTypeID;";

            return await _connection.QuerySingleOrDefaultAsync<LessonType>(sql, new { LessonTypeID = id });
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            const string sql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE IsActive = 1
                ORDER BY DurationMinutes;";

            return await _connection.QueryAsync<LessonType>(sql);
        }

        public async Task<int> InsertAsync(LessonType lessonType)
        {
            const string sql = @"
                INSERT INTO LessonType (DurationMinutes, BasePricePerLesson, IsActive)
                VALUES (@DurationMinutes, @BasePricePerLesson, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, lessonType);
        }

        public async Task<bool> UpdateAsync(LessonType lessonType)
        {
            const string sql = @"
                UPDATE LessonType
                SET DurationMinutes    = @DurationMinutes,
                    BasePricePerLesson = @BasePricePerLesson,
                    IsActive           = @IsActive
                WHERE LessonTypeID = @LessonTypeID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, lessonType);
            return rowsAffected > 0;
        }
    }
}
