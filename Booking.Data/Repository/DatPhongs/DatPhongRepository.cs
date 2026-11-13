using Booking.Common.Shared.Enum.Booking;
using Booking.Common.Shared.Enum.Payment;
using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Booking.Data.Repository.DatPhongs
{
    public class DatPhongRepository : IDatPhongRepository
    {
        private readonly AppDbContext _context;
        public DatPhongRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task UpdateBookingStatus(DatPhong d)
        {
             _context.DatPhongs.Update(d);
            return Task.CompletedTask;
        }

        public Task<bool> IsBookingOwnedBy(Guid bookingId, Guid ownerId)
        {
            return _context.DatPhongs.AnyAsync(x => x.Id == bookingId &&
                x.KhachSan != null && x.KhachSan.NguoiTao == ownerId);
        }

            public Task<bool> IsRoomTypeForHotel(Guid roomTypeId, Guid hotelId)
            {
                return _context.LoaiPhongs.AnyAsync(roomType =>
                roomType.Id == roomTypeId && roomType.KhachSanId == hotelId);
            }

        public async Task<(List<DatPhong> Items, int TotalCount)> GetOwnerBookings(
            Guid ownerId,
            Guid khachSanId,
            string? maDatPhong,
            int pageIndex,
            int pageSize)
        {
            var query = _context.DatPhongs
                .AsNoTracking()

                .Include(x => x.KhachSan)

                .Include(x => x.ThanhToans)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.LoaiPhong)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.PhongDats)
                .ThenInclude(x => x.Phong)

                .Where(x =>
                    x.KhachSanId == khachSanId &&
                    x.KhachSan != null &&
                    x.KhachSan.NguoiTao == ownerId
                );

            if (!string.IsNullOrWhiteSpace(maDatPhong))
            {
                var search = maDatPhong.Trim();
                query = query.Where(x => x.MaDatPhong != null && x.MaDatPhong.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<DatPhong> Items, int TotalCount)> GetBookingHistory(
            Guid userId,
            string? maDatPhong,
            int pageIndex,
            int pageSize)
        {
            var query = _context.DatPhongs
                .AsNoTracking()

                .Include(x => x.KhachSan)

                .Include(x => x.ThanhToans)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.LoaiPhong)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.PhongDats)
                .ThenInclude(x => x.Phong)

                .Where(x =>
                    x.KhachHangId == userId
                );

            if (!string.IsNullOrWhiteSpace(maDatPhong))
            {
                var search = maDatPhong.Trim();
                query = query.Where(x => x.MaDatPhong != null && x.MaDatPhong.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Booking.Core.Model.Users.NguoiDung>> GetCustomerOptionsByHotelId(
            Guid hotelId,
            Guid ownerId)
        {
            return await (
                from booking in _context.DatPhongs.AsNoTracking()
                join profile in _context.NguoiDungs.AsNoTracking()
                    on booking.KhachHangId equals profile.UserLoginId
                where booking.KhachSanId == hotelId
                    && booking.KhachHangId.HasValue
                    && booking.KhachSan != null
                    && booking.KhachSan.NguoiTao == ownerId
                select new Booking.Core.Model.Users.NguoiDung
                {
                    Id = profile.UserLoginId!.Value,
                    FullName = profile.FullName
                })
                .GroupBy(x => x.Id)
                .Select(group => group.First())
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        public async Task<DatPhong> CheckIn(Guid id)
        {
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (datPhong == null)
            {
                throw new Exception("Không tìm thấy đặt phòng");
            }

            datPhong.TrangThai = TrangThaiDatPhong.DA_CHECK_IN.ToString();

            await _context.SaveChangesAsync();

            return datPhong;
        }

        public async Task<DatPhong> ConfirmBooking(Guid id)
        {
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (datPhong == null)
            {
                throw new Exception("Không tìm thấy đặt phòng");
            }

            datPhong.TrangThai = TrangThaiDatPhong.DA_XAC_NHAN.ToString();

            await _context.SaveChangesAsync();

            return datPhong;
        }

        public async Task<DatPhong> CreateBooking(DatPhong dp, List<ChiTietDatPhong> ctdp, List<PhongDat> phongDats)
        {
            // 1. Thêm Đặt phòng chính
            await _context.DatPhongs.AddAsync(dp);

            // 2. Thêm danh sách Chi tiết đặt phòng (dùng AddRangeAsync ngắn gọn hơn foreach)
            if (ctdp != null && ctdp.Any())
            {
                await _context.ChiTietDatPhongs.AddRangeAsync(ctdp);
            }

            // 3. Thêm danh sách Phòng đặt (gán cụ thể từng PhongId cho ChiTietDatPhongId)
            if (phongDats != null && phongDats.Any())
            {
                await _context.phongDats.AddRangeAsync(phongDats);
            }

            // Lưu ý: Không gọi SaveChangesAsync() ở đây nếu bạn đang sử dụng UnitOfWork 
            // vì UnitOfWork sẽ đảm nhận việc Commit Transaction ở lớp Service.

            return dp;
        }

        public async Task<(int DeletedCount, int SkippedReferencedCount)> DeleteExpiredQrBookings(
            DateTime createdBefore,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var expiredBookings = await _context.DatPhongs
                .Where(booking =>
                    booking.NgayTao < createdBefore &&
                    (booking.TrangThai == TrangThaiDatPhong.CHO_XAC_NHAN.ToString() ||
                     booking.TrangThai == TrangThaiDatPhong.DA_HUY.ToString() ||
                     booking.TrangThai == TrangThaiDatPhong.TU_CHOI.ToString()) &&
                    booking.ThanhToans.Any(payment =>
                        (payment.PhuongThuc == PhuongThucThanhToan.CHUYEN_KHOAN.ToString() ||
                         payment.PhuongThuc == "QR") &&
                        (payment.TrangThai == TrangThaiThanhToan.CHO_THANH_TOAN.ToString() ||
                         payment.TrangThai == TrangThaiThanhToan.DA_HUY.ToString())))
                .ToListAsync(cancellationToken);

            if (expiredBookings.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return (0, 0);
            }

            var bookingIds = expiredBookings.Select(booking => booking.Id).ToList();
            var referencedBookingIds = new HashSet<Guid>();

            referencedBookingIds.UnionWith(await _context.HoaHongs
                .Where(item => bookingIds.Contains(item.MaDatPhong))
                .Select(item => item.MaDatPhong)
                .Distinct()
                .ToListAsync(cancellationToken));
            referencedBookingIds.UnionWith(await _context.LichSuVis
                .Where(item => item.MaDatPhong.HasValue && bookingIds.Contains(item.MaDatPhong.Value))
                .Select(item => item.MaDatPhong!.Value)
                .Distinct()
                .ToListAsync(cancellationToken));
            referencedBookingIds.UnionWith(await _context.ChiTietChiTraKhachSans
                .Where(item => bookingIds.Contains(item.DatPhongId))
                .Select(item => item.DatPhongId)
                .Distinct()
                .ToListAsync(cancellationToken));

            var deletableBookings = expiredBookings
                .Where(booking => !referencedBookingIds.Contains(booking.Id))
                .ToList();
            if (deletableBookings.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return (0, expiredBookings.Count);
            }

            var deletableBookingIds = deletableBookings.Select(booking => booking.Id).ToList();
            var detailIds = await _context.ChiTietDatPhongs
                .Where(detail => detail.DatPhongId.HasValue &&
                    deletableBookingIds.Contains(detail.DatPhongId.Value))
                .Select(detail => detail.Id)
                .ToListAsync(cancellationToken);
            var roomAssignments = await _context.phongDats
                .Where(assignment => detailIds.Contains(assignment.ChiTietDatPhongId))
                .ToListAsync(cancellationToken);
            var bookingDetails = await _context.ChiTietDatPhongs
                .Where(detail => detailIds.Contains(detail.Id))
                .ToListAsync(cancellationToken);
            var payments = await _context.ThanhToans
                .Where(payment => payment.DatPhongId.HasValue &&
                    deletableBookingIds.Contains(payment.DatPhongId.Value))
                .ToListAsync(cancellationToken);

            _context.phongDats.RemoveRange(roomAssignments);
            _context.ChiTietDatPhongs.RemoveRange(bookingDetails);
            _context.ThanhToans.RemoveRange(payments);
            _context.DatPhongs.RemoveRange(deletableBookings);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return (deletableBookings.Count, expiredBookings.Count - deletableBookings.Count);
        }

        public async Task<DatPhong?> GetBookingById(Guid id)
        {
            return await _context.DatPhongs
                .Include(x => x.KhachSan)
                .Include(x => x.ThanhToans)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<DatPhong?> GetCustomerBookingById(Guid id, Guid customerId)
        {
            return await _context.DatPhongs
                .Include(x => x.KhachSan)
                .Include(x => x.ThanhToans)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.LoaiPhong)
                .FirstOrDefaultAsync(x => x.Id == id && x.KhachHangId == customerId);
        }

        public async Task<bool> CancelBooking(Guid id, Guid customerId)
        {
            var booking = await _context.DatPhongs
                .FirstOrDefaultAsync(x => x.Id == id && x.KhachHangId == customerId);
            if (booking == null ||
                (booking.TrangThai != TrangThaiDatPhong.CHO_XAC_NHAN.ToString() &&
                 booking.TrangThai != TrangThaiDatPhong.DA_XAC_NHAN.ToString()))
            {
                return false;
            }

            booking.TrangThai = TrangThaiDatPhong.DA_HUY.ToString();
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangeOwnerBookingStatus(Guid id, Guid ownerId, string status)
        {
            var booking = await _context.DatPhongs.FirstOrDefaultAsync(item =>
                item.Id == id && item.KhachSan != null && item.KhachSan.NguoiTao == ownerId);
            if (booking == null)
            {
                return false;
            }

            var canReject = status == TrangThaiDatPhong.TU_CHOI.ToString() &&
                booking.TrangThai == TrangThaiDatPhong.CHO_XAC_NHAN.ToString();
            var canCancel = status == TrangThaiDatPhong.DA_HUY.ToString() &&
                booking.TrangThai == TrangThaiDatPhong.DA_XAC_NHAN.ToString();
            if (!canReject && !canCancel)
            {
                return false;
            }

            booking.TrangThai = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(Guid hotelId, DateTime batDau, DateTime ketThuc, string trangThai)
        {
            var query = _context.DatPhongs
                .AsNoTracking()
                .Where(x =>
                    x.KhachSanId == hotelId &&
                    x.NgayTao >= batDau &&
                    x.NgayTao <= ketThuc
                );

            // Nếu truyền trạng thái thì lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            var datPhong = await query.CountAsync();

            var tongTien = await query
                .Select(x => (double?)x.TongTien)
                .SumAsync() ?? 0;

            return (datPhong, tongTien);
        }

        public async Task<(List<DatPhong> Items, int TotalCount)> GetBookingsByOwner(
            Guid ownerId, 
            Guid? hotelId, 
            Guid? customerId,
            string? trangThai, 
            DateTime? ngayBatDau,
            DateTime? ngayKetThuc,
            string? maDatPhong,
            int pageIndex,
            int pageSize)
        {
            var query = _context.DatPhongs
                .AsNoTracking()

                .Include(x => x.KhachSan)

                .Include(x => x.ThanhToans)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.LoaiPhong)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.PhongDats)
                .ThenInclude(x => x.Phong)

                .Where(x => x.KhachSan != null &&
                    (ownerId == Guid.Empty || x.KhachSan.NguoiTao == ownerId));
            // Lọc theo khách sạn (nếu có chọn)
            if (hotelId.HasValue && hotelId.Value != Guid.Empty)
            {
                query = query.Where(x => x.KhachSanId == hotelId.Value);
            }

            // Lọc theo khách hàng (nếu có chọn)
            if (customerId.HasValue && customerId.Value != Guid.Empty)
            {
                query = query.Where(x => x.KhachHangId == customerId.Value);
            }

            // Lọc theo trạng thái đặt phòng
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(maDatPhong))
            {
                var search = maDatPhong.Trim();
                query = query.Where(x => x.MaDatPhong != null && x.MaDatPhong.Contains(search));
            }

            if (ngayBatDau.HasValue)
            {
                var tuNgay = ngayBatDau.Value.Date;

                query = query.Where(x => x.NgayTao >= tuNgay);
            }

            if (ngayKetThuc.HasValue)
            {
                var denNgay = ngayKetThuc.Value.Date.AddDays(1);

                query = query.Where(x => x.NgayTao < denNgay);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
