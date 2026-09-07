using Booking.Core.Model.KhachSanImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.KhachSanImage
{
    public interface IKhachSanImageService
    {
        Task<IEnumerable<KhachSanImages>> GetHotelImages(List<Guid> ids);
        Task<IEnumerable<KhachSanImages>> GetListImageByKhachSanId(Guid khachSanId);
    }
}
