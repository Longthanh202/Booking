using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.LoaiPhongs;
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

        public async Task<List<LoaiPhong>> GetLoaiPhongByKhachSanId(Guid khachSanId)
        {
            try
            {
               return await _loaiPhongRepository.GetLoaiPhongByKhachSanId(khachSanId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  

                return new List<LoaiPhong>();
            }
        }

        public async Task<LoaiPhong> TaoLoaiPhong(LoaiPhong lp)
        {
            try
            {
                lp.Id = Guid.NewGuid();
                return await _loaiPhongRepository.TaoLoaiPhong(lp);
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
