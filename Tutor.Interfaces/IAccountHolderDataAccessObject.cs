using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IAccountHolderDataAccessObject
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<AccountHolder?> GetByEmailAsync(string email);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int> InsertAsync(AccountHolder accountHolder);
        Task<bool> UpdateAsync(AccountHolder accountHolder);
    }
}