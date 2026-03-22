
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderRepository : IAccountHolderRepository
    {
        private readonly IAccountHolderDataAccessObject _accountHolderService;
        private readonly ILogger<AccountHolderRepository> _logger;

        public AccountHolderRepository(IAccountHolderDataAccessObject accountHolderService, ILogger<AccountHolderRepository> logger)
        {
            _accountHolderService = accountHolderService;
            _logger = logger;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            return await _accountHolderService.GetAccountHolderAsync(id);
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            return await _accountHolderService.GetByTeacherAsync(teacherId);
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                return await _accountHolderService.InsertAsync(accountHolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert AccountHolder {FirstName} {LastName}",
                    accountHolder.FirstName, accountHolder.LastName);
                return null;
            }
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                return await _accountHolderService.UpdateAsync(accountHolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update AccountHolderID {AccountHolderID}",
                    accountHolder.AccountHolderID);
                return false;
            }
        }
    }
}
