using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IAccountHolderRepository
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<AccountHolder?> GetByEmailAsync(string email);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int?> AddAccountHolderAsync(AccountHolder accountHolder);
        Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder);
    }
}