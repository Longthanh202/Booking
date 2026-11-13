using Booking.Core.Model.HotelImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.HotelImages
{
    public interface IKhachSanImageRepository
    {
        Task<List<KhachSanAnh>> GetHotelImages(List<Guid> ids);
        Task<List<KhachSanAnh>> GetListImageByKhachSanId(Guid khachSanId);
        Task InsertKhachSanImage(Guid khachSanId, List<string> Url);
        Task<KhachSanAnh?> GetImageForOwner(long imageId, Guid hotelId, Guid ownerId);
        Task<bool> DeleteImageForOwner(long imageId, Guid hotelId, Guid ownerId);
    }
}
