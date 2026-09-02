using Booking.Core.Model.KhachSans;
using Booking.Service.Dtos.KhachSan;
using Booking.Service.Dtos.KhachSanDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.KhachSans
{
    public interface IKhachSanService
    {
        Task<KhachSanCreateResponse> TaoKhachSan(KhachSanCreateRequest ks, Guid nguoiTao);
        Task<FilterHotelResponseDto> LayDanhSachKhachSanAdminAsync(AdminFilterHotelRequestDto dto);
        Task<FilterHotelResponseDto> LayDanhSachKhachSanOwnerAsync(OwnerFilterHotelRequestDto dto, Guid ownerId);
        Task<FilterHotelResponseDto> FilterHotelsAsync(FilterHotelRequestDto dto);
        Task<KhachSanDetailResponse?> DetailKhachSan(Guid id);
    }
}
