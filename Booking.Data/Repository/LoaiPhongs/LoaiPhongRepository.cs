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
        public async Task<List<LoaiPhong>> GetLoaiPhongOwner(Guid khachSanId)
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

        public async Task<List<LoaiPhongHienThi>> GetLoaiPhongByKhachSanId(
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
                    x.TrangThai == TrangThaiPhong.DANG_SU_DUNG.ToString());

                int phongDangDat = 0;

                if (ngayNhan.HasValue && ngayTra.HasValue)
                {
                    phongDangDat = await _context.phongDats
                        .Where(pd =>
                            pd.ChiTietDatPhong.LoaiPhongId == loaiPhong.Id &&
                            pd.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.DA_HUY.ToString() &&
                            pd.ChiTietDatPhong.DatPhong.NgayNhanPhong < ngayTra &&
                            pd.ChiTietDatPhong.DatPhong.NgayTraPhong > ngayNhan)
                        .Select(pd => pd.PhongId)
                        .Distinct()
                        .CountAsync();
                }

                int soPhongTrong = tongPhong - phongDangDat;

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

        public async Task<LoaiPhong> TaoLoaiPhong(LoaiPhong lp)
        {
            await _context.LoaiPhongs.AddAsync(lp);
            await _context.SaveChangesAsync();
            return lp;
        }
    }
}
