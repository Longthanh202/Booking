using CloudinaryDotNet.Actions;
using Container_App.Core.Interface.Provinces;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.Provinces;
using Container_App.Data.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Provinces
{
    public class ProvinceService : IProvinceService
    {
        private readonly IStoredProcedureExecutor _executor;
        public ProvinceService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }
        public async Task<IEnumerable<Province>> GetProvinces()
        {
            try
            {
                return await _executor.QueryAsync<Province>("sp_GetProvince");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get list province: {ex.Message}");

                return Enumerable.Empty<Province>();
            }
        }
    }
}
