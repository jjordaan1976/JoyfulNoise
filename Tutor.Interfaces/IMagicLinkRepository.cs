using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IMagicLinkRepository
    {
        Task<MagicLink?> GetByTokenAsync(Guid token);
        Task<Guid?> CreateForStudentAsync(int studentId);
        Task<Guid?> CreateForAccountHolderAsync(int accountHolderId);
    }
}
