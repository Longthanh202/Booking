using Booking.Core.Model.ViKhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ViKhachSans
{
    public interface IViKhachSanRepository
    {
        Task<ViKhachSan?> LayTheoId(long id);

        Task<ViKhachSan?> LayTheoKhachSan(Guid khachSanId);

        Task Tao(ViKhachSan viKhachSan);

        Task CapNhat(ViKhachSan viKhachSan);
    }
}
