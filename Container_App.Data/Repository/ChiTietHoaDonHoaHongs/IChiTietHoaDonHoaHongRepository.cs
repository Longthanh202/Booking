using Container_App.Core.Model.ChiTietHoaDonHoaHongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.ChiTietHoaDonHoaHongs
{
    public interface IChiTietHoaDonHoaHongRepository
    {
        Task<ChiTietHoaDonHoaHong?> LayTheoId(long id);

        Task<IEnumerable<ChiTietHoaDonHoaHong>> LayTheoHoaDon(long hoaDonHoaHongId);

        Task<IEnumerable<ChiTietHoaDonHoaHong>> LayTheoHoaHong(long hoaHongId);

        Task Tao(ChiTietHoaDonHoaHong chiTiet);

        Task TaoDanhSach(IEnumerable<ChiTietHoaDonHoaHong> danhSach);

        Task Xoa(long id);
    }
}
