using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IDbConnection _connection;

        public StudentService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            const string sql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE StudentID = @StudentID;";

            return await _connection.QuerySingleOrDefaultAsync<Student>(sql, new { StudentID = id });
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE AccountHolderID = @AccountHolderID
                  AND IsActive        = 1
                ORDER BY LastName, FirstName;";

            return await _connection.QueryAsync<Student>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Student student)
        {
            const string sql = @"
                INSERT INTO Student
                    (AccountHolderID, FirstName, LastName, DateOfBirth, IsAccountHolder, IsActive)
                VALUES
                    (@AccountHolderID, @FirstName, @LastName, @DateOfBirth, @IsAccountHolder, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, student);
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            const string sql = @"
                UPDATE Student
                SET AccountHolderID = @AccountHolderID,
                    FirstName       = @FirstName,
                    LastName        = @LastName,
                    DateOfBirth     = @DateOfBirth,
                    IsAccountHolder = @IsAccountHolder,
                    IsActive        = @IsActive
                WHERE StudentID = @StudentID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, student);
            return rowsAffected > 0;
        }
    }
}
