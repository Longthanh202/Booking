using Booking.Core.Model.GiaPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.GiaPhongs
{
    public interface IGiaPhongRepository
    {
        Task<List<GiaPhong>> LayGiaPhongTheoDSKhachSanId(List<Guid> ids);
        Task<GiaPhong?> LayGiaPhongHienTai(Guid loaiPhongId);
        Task<List<GiaPhong>> GetPricesByRoomType(Guid roomTypeId, Guid ownerId);
        Task<GiaPhong?> CreatePrice(GiaPhong price, Guid ownerId);
    }
}
