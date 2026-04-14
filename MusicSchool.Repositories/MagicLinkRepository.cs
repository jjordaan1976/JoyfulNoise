using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class MagicLinkRepository : IMagicLinkRepository
    {
        private readonly IMagicLinkDataAccessObject _dao;
        private readonly ILogger<MagicLinkRepository> _logger;

        public MagicLinkRepository(IMagicLinkDataAccessObject dao, ILogger<MagicLinkRepository> logger)
        {
            _dao = dao;
            _logger = logger;
        }

        public async Task<MagicLink?> GetByTokenAsync(Guid token)
        {
            try
            {
                return await _dao.GetByTokenAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve MagicLink for token {Token}", token);
                return null;
            }
        }

        public async Task<Guid?> CreateForStudentAsync(int studentId)
        {
            try
            {
                var link = new MagicLink
                {
                    Token = Guid.NewGuid(),
                    LinkType = MagicLinkType.Student,
                    EntityID = studentId,
                    IsActive = true
                };
                await _dao.InsertAsync(link);
                return link.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create student magic link for StudentID {StudentID}", studentId);
                return null;
            }
        }

        public async Task<Guid?> CreateForAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var link = new MagicLink
                {
                    Token = Guid.NewGuid(),
                    LinkType = MagicLinkType.AccountHolder,
                    EntityID = accountHolderId,
                    IsActive = true
                };
                await _dao.InsertAsync(link);
                return link.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create account holder magic link for AccountHolderID {AccountHolderID}", accountHolderId);
                return null;
            }
        }
    }
}
