using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LoaiPhongs;
using Booking.Data.Connection;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Service.Dtos.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.LoaiPhongs
{
    public class LoaiPhongService : ILoaiPhongService
    {
        private readonly ILoaiPhongRepository _loaiPhongRepository;
        public LoaiPhongService(ILoaiPhongRepository loaiPhongRepository)
        {
            _loaiPhongRepository = loaiPhongRepository;
        }

        public async Task<List<LoaiPhongHienThi>> GetLoaiPhongByKhachSanId(GetLoaiPhongDto input)
        {
            return await _loaiPhongRepository.GetLoaiPhongByKhachSanId(input.khachSanId,
                input.soKhach, input.ngayNhan, input.ngayTra);
        }

        public async Task<List<LoaiPhong>> GetLoaiPhongOwner(Guid khachSanId)
        {
            return await _loaiPhongRepository.GetLoaiPhongOwner(khachSanId);
        }

        public async Task<int> TaoLoaiPhong(List<LoaiPhongRequest> lp)
        {
            int count = 0;

            foreach (var item in lp)
            {
                LoaiPhong input = new LoaiPhong
                {
                    Id = Guid.NewGuid(),
                    KhachSanId = item.KhachSanId,
                    TenLoaiPhong = item.TenLoaiPhong,
                    SoKhachToiDa = item.SoKhachToiDa,
                    KieuGiuong = item.KieuGiuong,
                    MoTa = item.MoTa,
                    NgayTao = DateTime.Now,
                };

                var result = await _loaiPhongRepository.TaoLoaiPhong(input);

                if (result)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
