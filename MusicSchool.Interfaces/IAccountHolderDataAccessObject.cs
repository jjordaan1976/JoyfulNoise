using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IAccountHolderDataAccessObject
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int> InsertAsync(AccountHolder accountHolder);
        Task<bool> UpdateAsync(AccountHolder accountHolder);
    }
}