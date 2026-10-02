using Booking.Core.Model.DatPhongs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ThanhToans
{
    public class ThanhToanRepository : IThanhToanRepository
    {
        private readonly AppDbContext _context;
        public ThanhToanRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ThanhToan> CreatePayment(ThanhToan t)
        {
            await _context.AddAsync(t);
            return t;
        }

        public async Task<ThanhToan?> LayTheoDatPhong(Guid datPhongId)
        {
            return await _context.ThanhToans
                .FirstOrDefaultAsync(x => x.DatPhongId == datPhongId);
        }

        public Task<ThanhToan?> GetForCustomer(Guid paymentId, Guid customerId)
        {
            return _context.ThanhToans
                .Include(x => x.DatPhong)
                .FirstOrDefaultAsync(x => x.Id == paymentId &&
                    x.DatPhong != null && x.DatPhong.KhachHangId == customerId);
        }

        public Task<ThanhToan?> GetForCustomerBooking(Guid bookingId, Guid customerId)
        {
            return _context.ThanhToans
                .Include(x => x.DatPhong)
                .ThenInclude(x => x!.KhachSan)
                .FirstOrDefaultAsync(x => x.DatPhongId == bookingId &&
                    x.DatPhong != null && x.DatPhong.KhachHangId == customerId);
        }

        public Task<List<ThanhToan>> GetAll()
        {
            return _context.ThanhToans
                .Include(x => x.DatPhong)
                .OrderByDescending(x => x.ThoiGianThanhToan)
                .ToListAsync();
        }

        public async Task<ThanhToan?> UpdateStatus(Guid paymentId, string status)
        {
            var payment = await _context.ThanhToans.FirstOrDefaultAsync(x => x.Id == paymentId);
            if (payment == null)
            {
                return null;
            }

            payment.TrangThai = status;
            if (status == Booking.Common.Shared.Enum.Payment.TrangThaiThanhToan.DA_THANH_TOAN.ToString() ||
                status == Booking.Common.Shared.Enum.Payment.TrangThaiThanhToan.DA_HOAN_TIEN.ToString())
            {
                payment.ThoiGianThanhToan = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
