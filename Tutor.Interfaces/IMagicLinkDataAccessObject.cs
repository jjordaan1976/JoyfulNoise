using Tutor.Data.Models;

namespace Tutor.Data.Interfaces
{
    public interface IMagicLinkDataAccessObject
    {
        Task<MagicLink?> GetByTokenAsync(Guid token);
        Task<int> InsertAsync(MagicLink link);
        Task<bool> DeactivateAsync(int magicLinkId);
    }
}
