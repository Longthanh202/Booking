using Container_App.Core.Model.RefreshTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.RefreshTokens
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> InsertRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> CheckStatusefreshToken(string token);
    }
}
