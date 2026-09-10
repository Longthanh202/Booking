using Booking.Core.Model.RefreshTokens;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.RefreshTokens
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<RefreshToken?> CheckStatusefreshToken(string token)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.Status == 1 &&
                x.Token == token &&
                x.CreatedDate.HasValue &&
                x.ExpiryDate.HasValue &&
                x.CreatedDate.Value > DateTime.Now.AddDays(-x.ExpiryDate.Value));
        }

        public async Task<RefreshToken> InsertRefreshToken(RefreshToken refreshToken)
        {
            await _context.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
    }
}
