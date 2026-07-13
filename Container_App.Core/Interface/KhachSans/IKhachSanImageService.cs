using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Interface.KhachSans
{
    public interface IKhachSanImageService
    {
        Task<IEnumerable<KhachSanImages>> GetHotelImages(List<Guid> ids);
        Task<IEnumerable<KhachSanImages>> GetListImageByKhachSanId(Guid khachSanId);
    }
}
