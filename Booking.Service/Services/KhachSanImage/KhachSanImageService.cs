using Booking.Core.Model.KhachSanImage;
using Booking.Data.Connection;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.KhachSanImage
{
    public class KhachSanImageService : IKhachSanImageService
    {
        private readonly IStoredProcedureExecutor _executor;
        public KhachSanImageService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }
        public async Task<IEnumerable<KhachSanAnh>> GetHotelImages(List<Guid> ids)
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
            return await _executor.QueryAsync<KhachSanAnh>("sp_GetHotelImages", arr);
        }

        public async Task<IEnumerable<KhachSanAnh>> GetListImageByKhachSanId(Guid khachSanId)
        {
            var arr = new[]
            {
                new SqlParameter("@KhachSanId", khachSanId)
            };
            return await _executor.QueryAsync<KhachSanAnh>("sp_GetListImageByKhachSanId", arr);
        }
    }
}
