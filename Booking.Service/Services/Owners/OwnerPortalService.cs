using Booking.Common.Shared.Enum.Booking;
using Booking.Common.Shared.Enum.Hotel;
using Booking.Core.Model.Phongs;
using Booking.Data.DBContext;
using Booking.Service.Dtos.Owners;
using Microsoft.EntityFrameworkCore;

namespace Booking.Service.Services.Owners
{
    public class OwnerPortalService : IOwnerPortalService
    {
        private readonly AppDbContext _context;

        public OwnerPortalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OwnerDashboardResponse?> GetDashboard(Guid ownerId, Guid? hotelId)
        {
            if (hotelId.HasValue && !await OwnsHotel(ownerId, hotelId.Value))
            {
                return null;
            }

            var bookings = _context.DatPhongs.AsNoTracking()
                .Where(booking => booking.KhachSan != null && booking.KhachSan.NguoiTao == ownerId);
            if (hotelId.HasValue)
            {
                bookings = bookings.Where(booking => booking.KhachSanId == hotelId.Value);
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);
            var cancelledStatuses = new[]
            {
                TrangThaiDatPhong.DA_HUY.ToString(),
                TrangThaiDatPhong.TU_CHOI.ToString()
            };
            var completedStatus = TrangThaiDatPhong.DA_CHECK_OUT.ToString();
            var rentableStatuses = new[]
            {
                TrangThaiPhong.SAN_SANG.ToString(),
                TrangThaiPhong.DANG_SU_DUNG.ToString()
            };

            var roomQuery = _context.Phongs.AsNoTracking()
                .Where(room => room.LoaiPhong != null && room.LoaiPhong.KhachSan != null &&
                    room.LoaiPhong.KhachSan.NguoiTao == ownerId);
            if (hotelId.HasValue)
            {
                roomQuery = roomQuery.Where(room => room.LoaiPhong!.KhachSanId == hotelId.Value);
            }

            var totalRooms = await roomQuery.CountAsync(room => rentableStatuses.Contains(room.TrangThai!));
            var occupiedRoomIds = _context.phongDats.AsNoTracking()
                .Where(link => link.ChiTietDatPhong != null && link.ChiTietDatPhong.DatPhong != null &&
                    link.Phong.LoaiPhong != null && link.Phong.LoaiPhong.KhachSan != null &&
                    link.Phong.LoaiPhong.KhachSan.NguoiTao == ownerId &&
                    link.ChiTietDatPhong.DatPhong.NgayNhanPhong < tomorrow &&
                    link.ChiTietDatPhong.DatPhong.NgayTraPhong > today &&
                    !cancelledStatuses.Contains(link.ChiTietDatPhong.DatPhong.TrangThai!));
            if (hotelId.HasValue)
            {
                occupiedRoomIds = occupiedRoomIds.Where(link => link.Phong.LoaiPhong!.KhachSanId == hotelId.Value);
            }

            var occupiedRooms = await occupiedRoomIds.Select(link => link.PhongId).Distinct().CountAsync();
            var completedBookings = bookings.Where(booking => booking.TrangThai == completedStatus);

            return new OwnerDashboardResponse
            {
                TotalBookings = await bookings.CountAsync(),
                TodayBookings = await bookings.CountAsync(booking =>
                    booking.NgayNhanPhong >= today && booking.NgayNhanPhong < tomorrow),
                UpcomingBookings = await bookings.CountAsync(booking =>
                    booking.NgayNhanPhong >= tomorrow && !cancelledStatuses.Contains(booking.TrangThai!)),
                CompletedBookings = await completedBookings.CountAsync(),
                CancelledBookings = await bookings.CountAsync(booking => cancelledStatuses.Contains(booking.TrangThai!)),
                TotalRooms = totalRooms,
                OccupiedRoomsToday = occupiedRooms,
                OccupancyRateToday = totalRooms == 0 ? 0 : Math.Round(occupiedRooms * 100m / totalRooms, 2),
                RevenueToday = await completedBookings
                    .Where(booking => booking.NgayTao >= today && booking.NgayTao < tomorrow)
                    .SumAsync(booking => booking.TongTien ?? 0m),
                RevenueThisMonth = await completedBookings
                    .Where(booking => booking.NgayTao >= monthStart && booking.NgayTao < tomorrow)
                    .SumAsync(booking => booking.TongTien ?? 0m),
                RevenueThisYear = await completedBookings
                    .Where(booking => booking.NgayTao >= yearStart && booking.NgayTao < tomorrow)
                    .SumAsync(booking => booking.TongTien ?? 0m)
            };
        }

        public async Task<OwnerRevenueReportResponse?> GetRevenueReport(
            Guid ownerId,
            Guid? hotelId,
            DateTime from,
            DateTime to,
            string groupBy)
        {
            if (from >= to || (to - from).TotalDays > 1096 ||
                !new[] { "day", "month", "year" }.Contains(groupBy, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            if (hotelId.HasValue && !await OwnsHotel(ownerId, hotelId.Value))
            {
                return null;
            }

            var query = _context.DatPhongs.AsNoTracking()
                .Where(booking => booking.KhachSan != null && booking.KhachSan.NguoiTao == ownerId &&
                    booking.NgayTao >= from && booking.NgayTao < to);
            if (hotelId.HasValue)
            {
                query = query.Where(booking => booking.KhachSanId == hotelId.Value);
            }

            var bookings = await query.Select(booking => new
            {
                booking.NgayTao,
                booking.TrangThai,
                booking.TongTien
            }).ToListAsync();

            string Period(DateTime? date) => (groupBy.ToLowerInvariant()) switch
            {
                "day" => date?.ToString("yyyy-MM-dd") ?? "unknown",
                "month" => date?.ToString("yyyy-MM") ?? "unknown",
                _ => date?.ToString("yyyy") ?? "unknown"
            };

            var cancelled = new[] { TrangThaiDatPhong.DA_HUY.ToString(), TrangThaiDatPhong.TU_CHOI.ToString() };
            var completed = TrangThaiDatPhong.DA_CHECK_OUT.ToString();
            var points = bookings.GroupBy(booking => Period(booking.NgayTao))
                .OrderBy(group => group.Key)
                .Select(group => new OwnerRevenuePoint
                {
                    Period = group.Key,
                    Bookings = group.Count(),
                    CancelledBookings = group.Count(booking => cancelled.Contains(booking.TrangThai)),
                    Revenue = group.Where(booking => booking.TrangThai == completed)
                        .Sum(booking => booking.TongTien ?? 0m)
                }).ToList();
            var cancelledCount = bookings.Count(booking => cancelled.Contains(booking.TrangThai));

            return new OwnerRevenueReportResponse
            {
                Data = points,
                TotalBookings = bookings.Count,
                CancelledBookings = cancelledCount,
                CancellationRate = bookings.Count == 0 ? 0 : Math.Round(cancelledCount * 100m / bookings.Count, 2),
                TotalRevenue = bookings.Where(booking => booking.TrangThai == completed)
                    .Sum(booking => booking.TongTien ?? 0m)
            };
        }

        public async Task<OwnerCalendarResponse?> GetCalendar(Guid ownerId, Guid hotelId, DateTime from, DateTime to)
        {
            if (from >= to || (to - from).TotalDays > 366 || !await OwnsHotel(ownerId, hotelId))
            {
                return null;
            }

            var bookings = await _context.DatPhongs.AsNoTracking()
                .Where(booking => booking.KhachSanId == hotelId &&
                    booking.NgayNhanPhong < to && booking.NgayTraPhong > from)
                .Include(booking => booking.KhachSan)
                .Include(booking => booking.ChiTietDatPhongs)
                    .ThenInclude(detail => detail.PhongDats)
                    .ThenInclude(link => link.Phong)
                .ToListAsync();

            var customerIds = bookings.Where(booking => booking.KhachHangId.HasValue)
                .Select(booking => booking.KhachHangId!.Value).Distinct().ToList();
            var customers = await _context.UserProfiles.AsNoTracking()
                .Where(profile => profile.UserLoginId.HasValue && customerIds.Contains(profile.UserLoginId.Value))
                .Select(profile => new { UserId = profile.UserLoginId!.Value, profile.FullName })
                .ToDictionaryAsync(profile => profile.UserId, profile => profile.FullName);

            var blocks = await _context.RoomAvailabilityBlocks.AsNoTracking()
                .Where(block => block.StartAt < to && block.EndAt > from &&
                    block.Room.LoaiPhong != null && block.Room.LoaiPhong.KhachSanId == hotelId)
                .Include(block => block.Room)
                .ToListAsync();

            return new OwnerCalendarResponse
            {
                Bookings = bookings.Select(booking => new OwnerCalendarBooking
                {
                    BookingId = booking.Id,
                    HotelId = booking.KhachSanId,
                    HotelName = booking.KhachSan?.TenKhachSan,
                    CustomerId = booking.KhachHangId,
                    CustomerName = booking.KhachHangId.HasValue && customers.TryGetValue(booking.KhachHangId.Value, out var name)
                        ? name
                        : null,
                    CheckIn = booking.NgayNhanPhong,
                    CheckOut = booking.NgayTraPhong,
                    Status = booking.TrangThai,
                    RoomNumbers = booking.ChiTietDatPhongs.SelectMany(detail => detail.PhongDats)
                        .Select(link => link.Phong?.SoPhong).ToList()
                }).ToList(),
                Blocks = blocks.Select(block => new RoomAvailabilityBlockResponse
                {
                    Id = block.Id,
                    RoomId = block.RoomId,
                    RoomNumber = block.Room.SoPhong,
                    StartAt = block.StartAt,
                    EndAt = block.EndAt,
                    Reason = block.Reason
                }).ToList()
            };
        }

        public async Task<RoomAvailabilityBlockResponse?> CreateRoomBlock(
            Guid ownerId,
            Guid roomId,
            CreateRoomAvailabilityBlockRequest request)
        {
            if (request.StartAt >= request.EndAt || request.StartAt < DateTime.Now ||
                !await OwnsRoom(ownerId, roomId))
            {
                return null;
            }

            var hasBooking = await _context.phongDats.AnyAsync(link =>
                link.PhongId == roomId && link.ChiTietDatPhong != null &&
                link.ChiTietDatPhong.DatPhong != null &&
                link.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.DA_HUY.ToString() &&
                link.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.TU_CHOI.ToString() &&
                link.ChiTietDatPhong.DatPhong.NgayNhanPhong < request.EndAt &&
                link.ChiTietDatPhong.DatPhong.NgayTraPhong > request.StartAt);
            var hasBlock = await _context.RoomAvailabilityBlocks.AnyAsync(block =>
                block.RoomId == roomId && block.StartAt < request.EndAt && block.EndAt > request.StartAt);
            if (hasBooking || hasBlock)
            {
                return null;
            }

            var block = new RoomAvailabilityBlock
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                Reason = request.Reason?.Trim(),
                CreatedBy = ownerId,
                CreatedAt = DateTime.UtcNow
            };
            await _context.RoomAvailabilityBlocks.AddAsync(block);
            await _context.SaveChangesAsync();

            return new RoomAvailabilityBlockResponse
            {
                Id = block.Id,
                RoomId = block.RoomId,
                StartAt = block.StartAt,
                EndAt = block.EndAt,
                Reason = block.Reason
            };
        }

        public async Task<bool> DeleteRoomBlock(Guid ownerId, Guid blockId)
        {
            var block = await _context.RoomAvailabilityBlocks.FirstOrDefaultAsync(item =>
                item.Id == blockId && item.Room.LoaiPhong != null &&
                item.Room.LoaiPhong.KhachSan != null && item.Room.LoaiPhong.KhachSan.NguoiTao == ownerId);
            if (block == null)
            {
                return false;
            }

            _context.RoomAvailabilityBlocks.Remove(block);
            await _context.SaveChangesAsync();
            return true;
        }

        private Task<bool> OwnsHotel(Guid ownerId, Guid hotelId) =>
            _context.KhachSans.AnyAsync(hotel => hotel.Id == hotelId && hotel.NguoiTao == ownerId);

        private Task<bool> OwnsRoom(Guid ownerId, Guid roomId) =>
            _context.Phongs.AnyAsync(room => room.Id == roomId && room.LoaiPhong != null &&
                room.LoaiPhong.KhachSan != null && room.LoaiPhong.KhachSan.NguoiTao == ownerId);
    }
}