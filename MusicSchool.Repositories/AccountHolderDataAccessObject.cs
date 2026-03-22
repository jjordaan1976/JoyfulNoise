using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderDataAccessObject : IAccountHolderDataAccessObject
    {
        private readonly IDbConnection _connection;

        public AccountHolderDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            const string sql = @"
                SELECT AccountHolderID,
                       TeacherID,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       BillingAddress,
                       IsActive,
                       CreatedAt
                FROM AccountHolder
                WHERE AccountHolderID = @AccountHolderID;";

            return await _connection.QuerySingleOrDefaultAsync<AccountHolder>(sql, new { AccountHolderID = id });
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            const string sql = @"
                SELECT AccountHolderID,
                       TeacherID,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       BillingAddress,
                       IsActive,
                       CreatedAt
                FROM AccountHolder
                WHERE TeacherID = @TeacherID
                  AND IsActive  = 1
                ORDER BY LastName, FirstName;";

            return await _connection.QueryAsync<AccountHolder>(sql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(AccountHolder accountHolder)
        {
            const string sql = @"
                INSERT INTO AccountHolder
                    (TeacherID, FirstName, LastName, Email, Phone, BillingAddress, IsActive)
                VALUES
                    (@TeacherID, @FirstName, @LastName, @Email, @Phone, @BillingAddress, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, accountHolder);
        }

        public async Task<bool> UpdateAsync(AccountHolder accountHolder)
        {
            const string sql = @"
                UPDATE AccountHolder
                SET TeacherID      = @TeacherID,
                    FirstName      = @FirstName,
                    LastName       = @LastName,
                    Email          = @Email,
                    Phone          = @Phone,
                    BillingAddress = @BillingAddress,
                    IsActive       = @IsActive
                WHERE AccountHolderID = @AccountHolderID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, accountHolder);
            return rowsAffected > 0;
        }
    }
}
