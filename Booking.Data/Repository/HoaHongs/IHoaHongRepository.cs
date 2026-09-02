using Booking.Core.Model.HoaHongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.HoaHongs
{
    public interface IHoaHongRepository
    {
        Task<HoaHong?> LayTheoId(long id);

        Task<HoaHong?> LayTheoDatPhong(Guid datPhongId);

        Task<IEnumerable<HoaHong>> LayTheoKhachSan(Guid khachSanId);
        
        Task<IEnumerable<HoaHong>> LayTheoOwnerId(Guid ownerId);

        Task<IEnumerable<HoaHong>> LayChuaThu();

        Task Tao(HoaHong hoaHong);

        Task CapNhat(HoaHong hoaHong);

        Task Xoa(long id);

        Task<IEnumerable<HoaHong>> LayHoaHongChoThu(Guid ksId, DateTime tuNgay, DateTime denNgay);
    }
}
