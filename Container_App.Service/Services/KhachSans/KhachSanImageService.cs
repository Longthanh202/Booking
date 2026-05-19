using Container_App.Core.Interface.KhachSans;
using Container_App.Core.Model.KhachSans;
using Container_App.Data.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.KhachSans
{
    public class KhachSanImageService : IKhachSanImageService
    {
        private readonly IStoredProcedureExecutor _executor;
        public KhachSanImageService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }
        public async Task<IEnumerable<KhachSanImages>> GetHotelImages(List<Guid> ids)
        {
            try
            {
                var tb = new DataTable();
                tb.Columns.Add("Id", typeof(Guid));
                foreach (var item in ids)
                {
                    tb.Rows.Add(item);
                }
                var arr = new[]
                {                  
                    new SqlParameter("@HotelIds", SqlDbType.Structured)
                    {
                        TypeName = "HotelIdList",
                        Value = tb
                    }
                };
                return await _executor.QueryAsync<KhachSanImages>("sp_GetHotelImages", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create KhachSan.");

                return Enumerable.Empty<KhachSanImages>();
            }
        }
    }
}
