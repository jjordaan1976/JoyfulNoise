
using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Data.Implementations
{
    public class AccountHolderRepository : IAccountHolderRepository
    {
        private readonly IAccountHolderDataAccessObject _accountHolderService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountHolderRepository> _logger;

        public AccountHolderRepository(
            IAccountHolderDataAccessObject accountHolderService,
            IUserRepository userRepository,
            ILogger<AccountHolderRepository> logger)
        {
            _accountHolderService = accountHolderService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            return await _accountHolderService.GetAccountHolderAsync(id);
        }

        public async Task<AccountHolder?> GetByEmailAsync(string email)
        {
            return await _accountHolderService.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            return await _accountHolderService.GetByTeacherAsync(teacherId);
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            var accountHolderId = await _accountHolderService.InsertAsync(accountHolder);

            await _userRepository.CreateUserAsync(new User
            {
                Email = accountHolder.Email,
                DisplayName = accountHolder.FullName,
                Role = UserRole.AccountHolder,
                AccountHolderID = accountHolderId
            });

            return accountHolderId;
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            return await _accountHolderService.UpdateAsync(accountHolder);
        }
    }
}
