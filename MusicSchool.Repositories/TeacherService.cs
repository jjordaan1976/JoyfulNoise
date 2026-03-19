using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly IDbConnection _connection;

        public TeacherService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            const string sql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE TeacherID = @TeacherID;";

            return await _connection.QuerySingleOrDefaultAsync<Teacher>(sql, new { TeacherID = id });
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            const string sql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE IsActive = 1
                ORDER BY Name;";

            return await _connection.QueryAsync<Teacher>(sql);
        }

        public async Task<int> InsertAsync(Teacher teacher)
        {
            const string sql = @"
                INSERT INTO Teacher (Name, Email, Phone, IsActive)
                VALUES (@Name, @Email, @Phone, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, teacher);
        }

        public async Task<bool> UpdateAsync(Teacher teacher)
        {
            const string sql = @"
                UPDATE Teacher
                SET Name     = @Name,
                    Email    = @Email,
                    Phone    = @Phone,
                    IsActive = @IsActive
                WHERE TeacherID = @TeacherID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, teacher);
            return rowsAffected > 0;
        }
    }
}
