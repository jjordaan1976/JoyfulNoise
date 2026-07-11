using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class StudentDataAccessObject : IStudentDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       Email,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE StudentID = @StudentID;";

        public static readonly string GetByEmailSql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       Email,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE Email = @Email;";

        public static readonly string GetByAccountHolderSql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       Email,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE AccountHolderID = @AccountHolderID
                  AND IsActive        = 1
                ORDER BY LastName, FirstName;";

        public static readonly string InsertSql = @"
                INSERT INTO Student
                    (AccountHolderID, FirstName, LastName, Email, DateOfBirth, IsAccountHolder, IsActive)
                VALUES
                    (@AccountHolderID, @FirstName, @LastName, @Email, @DateOfBirth, @IsAccountHolder, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateSql = @"
                UPDATE Student
                SET AccountHolderID = @AccountHolderID,
                    FirstName       = @FirstName,
                    LastName        = @LastName,
                    Email           = @Email,
                    DateOfBirth     = @DateOfBirth,
                    IsAccountHolder = @IsAccountHolder,
                    IsActive        = @IsActive
                WHERE StudentID = @StudentID;";

        public StudentDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<Student>(GetByIdSql, new { StudentID = id });
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _connection.QuerySingleOrDefaultAsync<Student>(GetByEmailSql, new { Email = email });
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _connection.QueryAsync<Student>(GetByAccountHolderSql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Student student)
        {
            return await _connection.ExecuteScalarAsync<int>(InsertSql, student);
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateSql, student);
            return rowsAffected > 0;
        }
    }
}
