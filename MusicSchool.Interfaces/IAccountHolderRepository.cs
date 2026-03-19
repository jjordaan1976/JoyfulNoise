using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IAccountHolderRepository
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int?> AddAccountHolderAsync(AccountHolder accountHolder);
        Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder);
    }
}