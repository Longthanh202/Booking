using Booking.Core.Model.KhachSans;
using Booking.Service.Dtos.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.KhachSans
{
    public interface IKhachSanService
    {
        Task<CreateHotelResponse> CreateHotel(CreateHotelRequest ks, Guid nguoiTao);
        Task<HotelFilterResponse> LayDanhSachKhachSanAdminAsync(AdminHotelFilterRequest dto);
        Task<HotelFilterResponse> LayDanhSachKhachSanOwnerAsync(OwnerHotelFilterRequest dto, Guid ownerId);
        Task<HotelFilterResponse> FilterHotelsAsync(HotelFilterRequest dto);
        Task<HotelDetailsResponse?> GetHotelDetails(Guid id);
    }
}
