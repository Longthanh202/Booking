using Booking.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Phongs
{
    public interface IPhongRepository
    {
        Task<Phong> TaoPhong(Phong p);
        Task<List<Phong>> LayDanhSachPhongTrong(Guid loaiPhongId, DateTime? ngayNhan, DateTime? ngayTra);
    }
}
