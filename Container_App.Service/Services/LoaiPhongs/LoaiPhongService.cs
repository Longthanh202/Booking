using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Service.Dtos.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.LoaiPhongs
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
            try
            {
               return await _loaiPhongRepository.GetLoaiPhongByKhachSanId(input.khachSanId,
                   input.soKhach, input.ngayNhan, input.ngayTra);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  

                return new List<LoaiPhongHienThi>();
            }
        }

        public async Task<List<LoaiPhong>> GetLoaiPhongOwner(Guid khachSanId)
        {
            return await _loaiPhongRepository.GetLoaiPhongOwner(khachSanId);
        }

        public async Task<LoaiPhong> TaoLoaiPhong(LoaiPhongRequest lp)
        {
            try
            {
                LoaiPhong input = new LoaiPhong
                {
                    Id = Guid.NewGuid(),
                    KhachSanId = lp.KhachSanId,
                    TenLoaiPhong = lp.TenLoaiPhong,
                    SoKhachToiDa = lp.SoKhachToiDa,
                    KieuGiuong = lp.KieuGiuong,
                    MoTa = lp.MoTa,
                    NgayTao = DateTime.Now,
                };
                return await _loaiPhongRepository.TaoLoaiPhong(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create LoaiPhong.");

                return null;
            }
        }
    }
}
