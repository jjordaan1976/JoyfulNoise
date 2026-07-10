using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetByEmailAsync(string email);
        Task<int?> CreateUserAsync(User user);
    }
}
