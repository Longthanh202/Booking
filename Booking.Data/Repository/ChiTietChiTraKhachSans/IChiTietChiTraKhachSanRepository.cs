using Booking.Core.Model.ChiTietChiTraKhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ChiTietChiTraKhachSans
{
    public interface IChiTietChiTraKhachSanRepository
    {
        Task<ChiTietChiTraKhachSan?> LayTheoId(long id);

        Task<IEnumerable<ChiTietChiTraKhachSan>> LayTheoChiTra(long chiTraKhachSanId);

        Task<IEnumerable<ChiTietChiTraKhachSan>> LayTheoDatPhong(Guid datPhongId);

        Task Tao(ChiTietChiTraKhachSan chiTietChiTraKhachSan);

        Task TaoDanhSach(IEnumerable<ChiTietChiTraKhachSan> danhSach);

        Task Xoa(long id);
    }
}
