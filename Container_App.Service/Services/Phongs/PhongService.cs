using Container_App.Common.Shared.Enum.Hotel;
using Container_App.Core.Model.Phongs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Phongs;
using Container_App.Service.Dtos.Phongs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Phongs
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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create Phong.");

                return null;
            }
        }
    }
}
