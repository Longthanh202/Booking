using CloudinaryDotNet.Actions;
using Container_App.Core.Interface.Banners;
using Container_App.Core.Model.Banners;
using Container_App.Core.Model.KhachSans;
using Container_App.Data.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Banners
{
    public class BannerService : IBannerService
    {
        private readonly IStoredProcedureExecutor _executor;
        public BannerService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }

        public async Task<IEnumerable<Banner>> GetBannerIsActive()
        {
            try
            {              
                return await _executor.QueryAsync<Banner>("sp_GetBannerIsACtive");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get list GetBannerIsActive: {ex.Message}");

                return Enumerable.Empty<Banner>();
            }
        }

        public async Task<IEnumerable<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow)
        {
            try
            {
                var arr = new SqlParameter[]
                {
                    new SqlParameter("@Keyword",   SqlDbType.NVarChar, 255) { Value = (object?)keyword   ?? DBNull.Value },
                    new SqlParameter("@IsActive",  SqlDbType.NVarChar, 255) { Value = isActive },                    
                    new SqlParameter("@StartRow",  SqlDbType.Int)           { Value = startRow },
                    new SqlParameter("@EndRow",    SqlDbType.Int)           { Value = endRow }
                };
                return await _executor.QueryAsync<Banner>("sp_GetAllBanner", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get list GetAllBanner: {ex.Message}");

                return Enumerable.Empty<Banner>();
            }
        }

        public async Task<int> InsertBanner(Banner banner)
        {
            try
            {               
                var arr = new[]
                {
                    new SqlParameter("@Title", banner.Title),
                    new SqlParameter("@Subtitle", banner.Subtitle),
                    new SqlParameter("@Url", banner.Url),
                    new SqlParameter("@IsActive", banner.IsActive),

                };
                return await _executor.ExecuteAsync("sp_InsertBanner", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create Banner.");

                return -1;
            }
        }
    }
}
