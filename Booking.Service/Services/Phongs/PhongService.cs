using Booking.Common.Shared.Enum.Hotel;
using Booking.Core.Model.Phongs;
using Booking.Data.Connection;
using Booking.Data.Repository.Phongs;
using Booking.Service.Dtos.Phongs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Phongs
{
    public class PhongService : IPhongService
    {
        private readonly IPhongRepository _phongRepository;
        public PhongService(IPhongRepository phongRepository)
        {
           _phongRepository = phongRepository;
        }
        public async Task<Phong> TaoPhong(PhongRequest p)
        {
            Phong phong = new Phong
            {
                Id = Guid.NewGuid(),
                LoaiPhongId = p.LoaiPhongId,
                SoPhong = p.SoPhong,
                Tang = p.Tang,
                TrangThai = TrangThaiPhong.DANG_SU_DUNG.ToString(),
            };
            return await _phongRepository.TaoPhong(phong);
        }
    }
}
