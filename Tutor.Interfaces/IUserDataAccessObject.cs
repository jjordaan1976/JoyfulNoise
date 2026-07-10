using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IUserDataAccessObject
    {
        Task<IEnumerable<User>> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(string email, UserRole role);
        Task<int> InsertAsync(User user);
    }
}
