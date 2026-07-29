using Container_App.Core.Model.GiaPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.GiaPhongs
{
    public interface IGiaPhongRepository
    {
        Task<List<GiaPhong>> LayGiaPhongTheoDSKhachSanId(List<Guid> ids);
        Task<GiaPhong?> LayGiaPhongHienTai(Guid loaiPhongId);
    }
}
