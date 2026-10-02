using Booking.Common.Shared.Enum.Booking;
using Booking.Common.Shared.Enum.Hotel;
using Booking.Core.Model.LoaiPhongs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.LoaiPhongs
{
    public class LoaiPhongRepository : ILoaiPhongRepository
    {
        private readonly AppDbContext _context;
        public LoaiPhongRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<LoaiPhong>> GetOwnerRoomTypes(Guid khachSanId)
        {
            return await _context.LoaiPhongs
                .Where(x => x.KhachSanId == khachSanId)
                .Select(x => new LoaiPhong
                {
                    Id = x.Id,
                    TenLoaiPhong = x.TenLoaiPhong,
                    SoKhachToiDa = x.SoKhachToiDa,
                    KieuGiuong = x.KieuGiuong,
                    MoTa = x.MoTa,
                })
                .ToListAsync();
        }

        public async Task<LoaiPhong?> GetRoomTypeForOwner(Guid roomTypeId, Guid ownerId)
        {
            return await _context.LoaiPhongs
                .Include(x => x.KhachSan)
                .FirstOrDefaultAsync(x => x.Id == roomTypeId &&
                    x.KhachSan != null && x.KhachSan.NguoiTao == ownerId);
        }

        public async Task<List<LoaiPhongHienThi>> GetRoomTypesByHotelId(
            Guid khachSanId,
            int soKhach,
            DateTime? ngayNhan,
            DateTime? ngayTra)
        {
            int soDem = 1;

            if (ngayNhan.HasValue && ngayTra.HasValue)
            {
                soDem = (ngayTra.Value.Date - ngayNhan.Value.Date).Days;

                if (soDem <= 0)
                    throw new Exception("Ngày trả phòng phải lớn hơn ngày nhận phòng.");
            }

            var loaiPhongs = await _context.LoaiPhongs
                .Where(x => x.KhachSanId == khachSanId)
                .ToListAsync();

            var result = new List<LoaiPhongHienThi>();

            foreach (var loaiPhong in loaiPhongs)
            {
                int tongPhong = await _context.Phongs
                    .CountAsync(x => x.LoaiPhongId == loaiPhong.Id &&
                    (x.TrangThai == TrangThaiPhong.DANG_SU_DUNG.ToString() ||
                     x.TrangThai == TrangThaiPhong.SAN_SANG.ToString()));

                var busyRoomIds = new List<Guid>();

                if (ngayNhan.HasValue && ngayTra.HasValue)
                {
                    var bookedRoomIds = await _context.phongDats
                        .Where(pd =>
                            pd.ChiTietDatPhong.LoaiPhongId == loaiPhong.Id &&
                            pd.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.DA_HUY.ToString() &&
                            pd.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.TU_CHOI.ToString() &&
                            pd.ChiTietDatPhong.DatPhong.NgayNhanPhong < ngayTra &&
                            pd.ChiTietDatPhong.DatPhong.NgayTraPhong > ngayNhan)
                        .Select(pd => pd.PhongId)
                        .Distinct()
                        .ToListAsync();
                    var blockedRoomIds = await _context.RoomAvailabilityBlocks
                        .Where(block => block.Room.LoaiPhongId == loaiPhong.Id &&
                            block.StartAt < ngayTra && block.EndAt > ngayNhan)
                        .Select(block => block.RoomId)
                        .ToListAsync();
                    busyRoomIds = bookedRoomIds.Union(blockedRoomIds).ToList();
                }

                int soPhongTrong = tongPhong - busyRoomIds.Count;

                decimal giaMoiDem = 0;

                var gia = await _context.GiaPhongs
                    .Where(x =>
                        x.LoaiPhongId == loaiPhong.Id &&
                        x.IsActive == true)
                    .FirstOrDefaultAsync();

                if (gia != null)
                {
                    giaMoiDem = gia.Gia;
                }

                result.Add(new LoaiPhongHienThi
                {
                    Id = loaiPhong.Id,
                    TenLoaiPhong = loaiPhong.TenLoaiPhong,
                    SoKhachToiDa = loaiPhong.SoKhachToiDa.Value,
                    KieuGiuong = loaiPhong.KieuGiuong,
                    MoTa = loaiPhong.MoTa,

                    GiaMoiDem = giaMoiDem,
                    TongTien = giaMoiDem * soDem,

                    SoPhongTrong = soPhongTrong,

                    DuChoSoKhach = soPhongTrong * loaiPhong.SoKhachToiDa >= soKhach
                });
            }

            return result;
        }

        public async Task<bool> CreateRoomType(LoaiPhong lp)
        {
            try
            {
                await _context.LoaiPhongs.AddAsync(lp);

                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch
            {
                return false;
            }
        }

    }
}
