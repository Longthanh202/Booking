using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Auths
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DangNhapNgoai?> GetExternalLoginAsync(string provider, string providerKey)
        {
            return await _context.Set<DangNhapNgoai>()
                .Include(el => el.TaiKhoan)
                    .ThenInclude(ul => ul.NguoiDung)
                .FirstOrDefaultAsync(el => el.Provider == provider && el.ProviderKey == providerKey);
        }

        public async Task<NguoiDung?> GetProfileByEmailAsync(string email)
        {
            return await _context.NguoiDungs
                .Include(p => p.TaiKhoan)
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task CreateUserWithSocialAsync(TaiKhoan userLogin, NguoiDung userProfile, NguoiDung_VaiTro userRole, DangNhapNgoai externalLogin)
        {
            await _context.TaiKhoans.AddAsync(userLogin);
            await _context.NguoiDungs.AddAsync(userProfile);
            await _context.Set<NguoiDung_VaiTro>().AddAsync(userRole);
            await _context.Set<DangNhapNgoai>().AddAsync(externalLogin);
        }

        public async Task AddExternalLoginAsync(DangNhapNgoai externalLogin)
        {
            await _context.Set<DangNhapNgoai>().AddAsync(externalLogin);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
