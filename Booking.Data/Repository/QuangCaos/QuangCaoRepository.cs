using Booking.Core.Model.QuangCaos;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.QuangCaos
{
    public class QuangCaoRepository : IQuangCaoRepository
    {
        private readonly AppDbContext _context;

        public QuangCaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<GoiQuangCao>> GetActivePackages() =>
            _context.GoiQuangCaos.Where(x => x.IsActive == 1).OrderBy(x => x.GiaTien).ToListAsync();

        public Task<GoiQuangCao?> GetActivePackage(int id) =>
            _context.GoiQuangCaos.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == 1);

        public Task<List<KhachSanQuangCao>> GetByOwner(Guid ownerId) =>
            _context.KhachSanQuangCaos
                .Include(x => x.KhachSan)
                .Include(x => x.GoiQuangCao)
                .Where(x => x.KhachSan != null && x.KhachSan.NguoiTao == ownerId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

        public Task<List<KhachSanQuangCao>> GetPendingApproval() =>
            _context.KhachSanQuangCaos
                .Include(x => x.KhachSan)
                .Include(x => x.GoiQuangCao)
                .Where(x => x.TrangThai == "CHO_DUYET")
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();

        public async Task<KhachSanQuangCao?> Create(KhachSanQuangCao campaign, Guid ownerId)
        {
            var ownsHotel = await _context.KhachSans.AnyAsync(x => x.Id == campaign.KhachSanId && x.NguoiTao == ownerId);
            if (!ownsHotel)
            {
                return null;
            }

            await _context.KhachSanQuangCaos.AddAsync(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }

        public Task<KhachSanQuangCao?> GetOwned(long id, Guid ownerId) =>
            _context.KhachSanQuangCaos
                .Include(x => x.GoiQuangCao)
                .Include(x => x.KhachSan)
                .FirstOrDefaultAsync(x => x.Id == id && x.KhachSan != null && x.KhachSan.NguoiTao == ownerId);

        public Task<KhachSanQuangCao?> GetById(long id) =>
            _context.KhachSanQuangCaos.Include(x => x.GoiQuangCao).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<KhachSanQuangCao?> Update(KhachSanQuangCao campaign)
        {
            _context.KhachSanQuangCaos.Update(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }
    }
}