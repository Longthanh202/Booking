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
        Task<IEnumerable<KhachSanAnh>> GetHotelImages(List<Guid> ids);
        Task<IEnumerable<KhachSanAnh>> GetListImageByKhachSanId(Guid khachSanId);
    }
}
