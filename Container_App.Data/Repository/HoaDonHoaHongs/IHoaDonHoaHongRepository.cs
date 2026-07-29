using Container_App.Core.Model.HoaDonHoaHongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.HoaDonHoaHongs
{
    public interface IHoaDonHoaHongRepository
    {
        Task<HoaDonHoaHong?> LayTheoId(long id);

        Task<IEnumerable<HoaDonHoaHong>> LayTheoKhachSan(Guid khachSanId);

        Task<IEnumerable<HoaDonHoaHong>> LayTheoTrangThai(string trangThai);

        Task<IEnumerable<HoaDonHoaHong>> LayTheoKhoangNgay(
            DateTime tuNgay,
            DateTime denNgay);

        Task Tao(HoaDonHoaHong hoaDon);

        Task CapNhat(HoaDonHoaHong hoaDon);

        Task Xoa(long id);
    }
}
