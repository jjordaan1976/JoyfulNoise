using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class TeacherDataAccessObject : ITeacherDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE TeacherID = @TeacherID;";

        public static readonly string GetAllActiveSql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE IsActive = 1
                ORDER BY Name;";

        public static readonly string InsertSql = @"
                INSERT INTO Teacher (Name, Email, Phone, IsActive)
                VALUES (@Name, @Email, @Phone, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateSql = @"
                UPDATE Teacher
                SET Name     = @Name,
                    Email    = @Email,
                    Phone    = @Phone,
                    IsActive = @IsActive
                WHERE TeacherID = @TeacherID;";

        public TeacherDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<Teacher>(GetByIdSql, new { TeacherID = id });
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            return await _connection.QueryAsync<Teacher>(GetAllActiveSql);
        }

        public async Task<int> InsertAsync(Teacher teacher)
        {
            return await _connection.ExecuteScalarAsync<int>(InsertSql, teacher);
        }

        public async Task<bool> UpdateAsync(Teacher teacher)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateSql, teacher);
            return rowsAffected > 0;
        }
    }
}
