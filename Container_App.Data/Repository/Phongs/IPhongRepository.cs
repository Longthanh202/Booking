using Container_App.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Phongs
{
    public interface IPhongRepository
    {
        Task<Phong> TaoPhong(Phong p);
        Task<List<Phong>> LayDanhSachPhongTrong(Guid loaiPhongId, DateTime? ngayNhan, DateTime? ngayTra);
    }
}
