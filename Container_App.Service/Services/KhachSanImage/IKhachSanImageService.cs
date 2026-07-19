using Container_App.Core.Model.KhachSanImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.KhachSanImage
{
    public interface IKhachSanImageService
    {
        Task<IEnumerable<KhachSanImages>> GetHotelImages(List<Guid> ids);
        Task<IEnumerable<KhachSanImages>> GetListImageByKhachSanId(Guid khachSanId);
    }
}
