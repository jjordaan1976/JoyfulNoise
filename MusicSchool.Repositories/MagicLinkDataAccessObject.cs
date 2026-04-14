using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class MagicLinkDataAccessObject : IMagicLinkDataAccessObject
    {
        private readonly IDbConnection _connection;

        public MagicLinkDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<MagicLink?> GetByTokenAsync(Guid token)
        {
            const string sql = @"
                SELECT MagicLinkID, Token, LinkType, EntityID, CreatedAt, ExpiresAt, IsActive
                FROM   MagicLink
                WHERE  Token = @Token;";

            return await _connection.QuerySingleOrDefaultAsync<MagicLink>(sql, new { Token = token });
        }

        public async Task<int> InsertAsync(MagicLink link)
        {
            const string sql = @"
                INSERT INTO MagicLink (Token, LinkType, EntityID, ExpiresAt, IsActive)
                VALUES (@Token, @LinkType, @EntityID, @ExpiresAt, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, link);
        }

        public async Task<bool> DeactivateAsync(int magicLinkId)
        {
            const string sql = @"
                UPDATE MagicLink
                SET    IsActive = 0
                WHERE  MagicLinkID = @MagicLinkID;";

            var rows = await _connection.ExecuteAsync(sql, new { MagicLinkID = magicLinkId });
            return rows > 0;
        }
    }
}
