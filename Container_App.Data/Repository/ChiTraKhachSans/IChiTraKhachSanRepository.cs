using Container_App.Core.Model.ChiTraKhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.ChiTraKhachSans
{
    public interface IChiTraKhachSanRepository
    {
        Task<ChiTraKhachSan?> LayTheoId(long id);

        Task<IEnumerable<ChiTraKhachSan>> LayTheoKhachSan(Guid khachSanId);

        Task<IEnumerable<ChiTraKhachSan>> LayTheoTrangThai(string trangThai);

        Task<IEnumerable<ChiTraKhachSan>> LayChoChiTra();

        Task Tao(ChiTraKhachSan chiTraKhachSan);

        Task CapNhat(ChiTraKhachSan chiTraKhachSan);

        Task Xoa(long id);
    }
}
