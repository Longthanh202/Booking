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
        private readonly IStoredProcedureExecutor _executor;
        public LoaiPhongService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }

        public async Task<IEnumerable<LoaiPhong>> GetLoaiPhongByKhachSanId(Guid khachSanId)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@KhachSanId", khachSanId),
                 
                };
                return await _executor.QueryAsync<LoaiPhong>("sp_GetLoaiPhongByKhachSanId", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when GetLoaiPhongByKhachSanId.");

                return Enumerable.Empty<LoaiPhong>();
            }
        }

        public async Task<int> TaoLoaiPhong(LoaiPhong lp)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@KhachSanId", lp.KhachSanId),
                    new SqlParameter("@TenLoaiPhong", lp.TenLoaiPhong),
                    new SqlParameter("@SoKhachToiDa", lp.SoKhachToiDa),
                    new SqlParameter("@KieuGiuong", lp.KieuGiuong),
                    new SqlParameter("@MoTa", lp.MoTa),                   
                };
                return await _executor.ExecuteAsync("sp_ThemLoaiPhong", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create LoaiPhong.");

                return -1;
            }
        }
    }
}
