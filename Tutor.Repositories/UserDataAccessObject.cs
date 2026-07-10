using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class UserDataAccessObject : IUserDataAccessObject
    {
        private readonly IDbConnection _connection;

        public UserDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<User>> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT UserID,
                       Email,
                       DisplayName,
                       Role,
                       TeacherID,
                       StudentID,
                       AccountHolderID,
                       IsActive,
                       CreatedAt
                FROM [User]
                WHERE Email    = @Email
                  AND IsActive = 1;";

            return await _connection.QueryAsync<User>(sql, new { Email = email });
        }

        public async Task<bool> ExistsAsync(string email, UserRole role)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM [User]
                WHERE Email = @Email
                  AND Role  = @Role;";

            var count = await _connection.ExecuteScalarAsync<int>(sql,
                new { Email = email, Role = role.ToString() });
            return count > 0;
        }

        public async Task<int> InsertAsync(User user)
        {
            const string sql = @"
                INSERT INTO [User]
                    (Email, DisplayName, Role, TeacherID, StudentID, AccountHolderID, IsActive)
                VALUES
                    (@Email, @DisplayName, @Role, @TeacherID, @StudentID, @AccountHolderID, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, new
            {
                user.Email,
                user.DisplayName,
                Role = user.Role.ToString(),
                user.TeacherID,
                user.StudentID,
                user.AccountHolderID,
                user.IsActive
            });
        }
    }
}
