using Container_App.Core.Model.DatPhongs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.DatPhongs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.DatPhongs
{
    public class DatPhongService : IDatPhongService
    {
        private readonly IStoredProcedureExecutor _executor;
        public DatPhongService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }
        public async Task<DatPhong> DatPhong(DatPhong dp, List<ChiTietDatPhong> ctdp)
        {
            try
            {
                var tb = new DataTable();
                tb.Columns.Add("LoaiPhongId", typeof(Guid));
                tb.Columns.Add("SoLuongPhong", typeof(int));
                tb.Columns.Add("GiaMoiDem", typeof(decimal));
                foreach (var item in ctdp)
                {
                    tb.Rows.Add(item.LoaiPhongId, item.SoLuongPhong, item.GiaMoiDem);
                }
                var arr = new[]
                {
                    new SqlParameter("@KhachHangId", dp.KhachHangId),
                    new SqlParameter("@KhachSanId", dp.KhachSanId),                   
                    new SqlParameter("@NgayNhanPhong", dp.NgayNhanPhong),
                    new SqlParameter("@NgayTraPhong", dp.NgayTraPhong),
                    new SqlParameter("@TongTien", dp.TongTien),
                    new SqlParameter("@DanhSachPhong", SqlDbType.Structured)
                    {
                        TypeName = "TVP_ChiTietDatPhong",
                        Value = tb
                    }
                };
                return await _executor.QuerySingleAsync<DatPhong>("sp_DatPhong", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create Dat phong.");

                return null;
            }
        }
    }
}
