using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class AccountHolderDataAccessObject : IAccountHolderDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
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

        public static readonly string GetByEmailSql = @"
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
                WHERE Email = @Email;";

        public static readonly string GetByTeacherSql = @"
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

        public static readonly string InsertSql = @"
                INSERT INTO AccountHolder
                    (TeacherID, FirstName, LastName, Email, Phone, BillingAddress, IsActive)
                VALUES
                    (@TeacherID, @FirstName, @LastName, @Email, @Phone, @BillingAddress, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string UpdateSql = @"
                UPDATE AccountHolder
                SET TeacherID      = @TeacherID,
                    FirstName      = @FirstName,
                    LastName       = @LastName,
                    Email          = @Email,
                    Phone          = @Phone,
                    BillingAddress = @BillingAddress,
                    IsActive       = @IsActive
                WHERE AccountHolderID = @AccountHolderID;";

        public AccountHolderDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<AccountHolder>(GetByIdSql, new { AccountHolderID = id });
        }

        public async Task<AccountHolder?> GetByEmailAsync(string email)
        {
            return await _connection.QuerySingleOrDefaultAsync<AccountHolder>(GetByEmailSql, new { Email = email });
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            return await _connection.QueryAsync<AccountHolder>(GetByTeacherSql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(AccountHolder accountHolder)
        {
            return await _connection.ExecuteScalarAsync<int>(InsertSql, accountHolder);
        }

        public async Task<bool> UpdateAsync(AccountHolder accountHolder)
        {
            var rowsAffected = await _connection.ExecuteAsync(UpdateSql, accountHolder);
            return rowsAffected > 0;
        }
    }
}
