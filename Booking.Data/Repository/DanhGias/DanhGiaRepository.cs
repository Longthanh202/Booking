using Booking.Common.Shared.Enum.Booking;
using Booking.Core.Model.DanhGias;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.DanhGias
{
    public class DanhGiaRepository : IDanhGiaRepository
    {
        private readonly AppDbContext _context;

        public DanhGiaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DanhGia?> Create(DanhGia review)
        {
            await _context.DanhGias.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public Task<List<DanhGia>> GetByHotel(Guid hotelId)
        {
            return _context.DanhGias
                .AsNoTracking()
                .Where(x => x.KhachSanId == hotelId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public Task<List<DanhGia>> GetByCustomer(Guid customerId)
        {
            return _context.DanhGias
                .AsNoTracking()
                .Where(x => x.KhachHangId == customerId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public Task<bool> CanReview(Guid hotelId, Guid customerId)
        {
            return _context.DatPhongs.AnyAsync(x =>
                x.KhachSanId == hotelId &&
                x.KhachHangId == customerId &&
                x.TrangThai == TrangThaiDatPhong.DA_CHECK_OUT.ToString());
        }

        public Task<bool> IsHotelOwnedBy(Guid hotelId, Guid ownerId)
        {
            return _context.KhachSans.AnyAsync(x => x.Id == hotelId && x.NguoiTao == ownerId);
        }

        public async Task<bool> RespondByOwner(Guid reviewId, Guid ownerId, string response)
        {
            var review = await _context.DanhGias.FirstOrDefaultAsync(item =>
                item.Id == reviewId && item.KhachSan != null && item.KhachSan.NguoiTao == ownerId);
            if (review == null)
            {
                return false;
            }

            review.OwnerResponse = response;
            review.OwnerResponseAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReportByOwner(Guid reviewId, Guid ownerId, string reason)
        {
            var review = await _context.DanhGias.FirstOrDefaultAsync(item =>
                item.Id == reviewId && item.KhachSan != null && item.KhachSan.NguoiTao == ownerId);
            if (review == null)
            {
                return false;
            }

            review.OwnerReportReason = reason;
            review.OwnerReportedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid reviewId)
        {
            var review = await _context.DanhGias.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null)
            {
                return false;
            }

            _context.DanhGias.Remove(review);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}