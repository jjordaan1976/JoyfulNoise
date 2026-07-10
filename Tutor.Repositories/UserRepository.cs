using Microsoft.Extensions.Logging;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Data.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserDataAccessObject _userService;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IUserDataAccessObject userService, ILogger<UserRepository> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetByEmailAsync(string email)
        {
            return await _userService.GetByEmailAsync(email);
        }

        public async Task<int?> CreateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                _logger.LogWarning("Skipped creating {Role} user for {DisplayName}: no email address",
                    user.Role, user.DisplayName);
                return null;
            }

            try
            {
                if (await _userService.ExistsAsync(user.Email, user.Role))
                {
                    _logger.LogInformation("User already exists for {Email} as {Role}", user.Email, user.Role);
                    return null;
                }

                return await _userService.InsertAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create {Role} user for {Email}", user.Role, user.Email);
                return null;
            }
        }
    }
}
