using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.TaiKhoanNganHangs
{
    public class TaiKhoanNganHangRepository : ITaiKhoanNganHangRepository
    {
        private readonly AppDbContext _context;

        public TaiKhoanNganHangRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<TaiKhoanNganHang>> GetByUser(Guid userId) =>
            _context.TaiKhoanNganHangs.Where(x => x.UserId == userId).ToListAsync();

        public Task<TaiKhoanNganHang?> GetOwned(Guid id, Guid userId) =>
            _context.TaiKhoanNganHangs.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        public async Task<TaiKhoanNganHang> Create(TaiKhoanNganHang account)
        {
            await _context.TaiKhoanNganHangs.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }
    }
}