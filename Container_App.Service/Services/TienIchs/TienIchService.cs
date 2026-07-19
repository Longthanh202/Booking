using Container_App.Core.Model.TienIchs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.TienIchs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.TienIchs
{
    public class TienIchService : ITienIchService
    {
        private readonly IStoredProcedureExecutor _executor;
        public TienIchService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }

        public async Task<IEnumerable<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@KhachSanId", khachSanId),
                };
                return await _executor.QueryAsync<TienIch>("sp_GetTienIchKhachSanByKhachSanId", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when GetTienIchKhachSanByKhachSanId.");

                return Enumerable.Empty<TienIch>();
            }
        }

        public async Task<int> ThemTienIch(TienIch tienIch)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@TenTienIch", tienIch.TenTienIch),
                    new SqlParameter("@Icon", tienIch.Icon)                    
                };
                return await _executor.ExecuteAsync("sp_ThemTienIch", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when Them Tien Ich.");

                return -1;
            }
        }
    }
}
