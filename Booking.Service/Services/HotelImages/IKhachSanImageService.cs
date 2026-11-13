using Booking.Core.Model.HotelImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.HotelImages
{
    public interface IKhachSanImageService
    {
        Task<IEnumerable<KhachSanAnh>> GetHotelImages(List<Guid> ids);
        Task<IEnumerable<KhachSanAnh>> GetListImageByKhachSanId(Guid khachSanId);
    }
}
