using Booking.Core.Model.RefreshTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.RefreshTokens
{
    public interface IRefreshTokenRepository
    {
        Task<TokenLamMoi> InsertRefreshToken(TokenLamMoi refreshToken);
        Task<TokenLamMoi> GetValidRefreshToken(string token);
    }
}
