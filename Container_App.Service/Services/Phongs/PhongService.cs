using Container_App.Core.Model.Phongs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Phongs;
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
        private readonly IStoredProcedureExecutor _executor;
        public PhongService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }
        public async Task<int> TaoPhong(Phong p)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@LoaiPhongId", p.LoaiPhongId),
                    new SqlParameter("@SoPhong", p.SoPhong),
                    new SqlParameter("@Tang", p.Tang),
                    new SqlParameter("@TrangThai", p.TrangThai),                   
                };
                return await _executor.ExecuteAsync("sp_ThemPhong", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create Phong.");

                return -1;
            }
        }
    }
}
