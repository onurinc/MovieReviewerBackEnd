using MR.DataAccessLayer.Entities;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Interfaces
{
    public interface IRefreshTokensRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> GetByRefreshToken(string RefreshToken);

        Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);

    }
}
