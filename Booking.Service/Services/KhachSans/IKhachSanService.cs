using Booking.Core.Model.KhachSans;
using Booking.Service.Dtos.Hotels;
using Booking.Service.Dtos.Common;
using Microsoft.AspNetCore.Http;
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
        Task<List<OptionDto>> GetHotelOptionsByOwnerId(Guid ownerId);
        Task<HotelFilterResponse> FilterHotelsAsync(HotelFilterRequest dto);
        Task<HotelDetailsResponse?> GetHotelDetails(Guid id);
        Task<bool> UpdateHotel(Guid hotelId, Guid ownerId, UpdateHotelRequest request);
        Task<bool> AddHotelImages(Guid hotelId, Guid ownerId, List<IFormFile> files);
        Task<bool> DeleteHotelImage(Guid hotelId, long imageId, Guid ownerId);
        Task<bool> UpdateStatus(Guid hotelId, string status);
    }
}
