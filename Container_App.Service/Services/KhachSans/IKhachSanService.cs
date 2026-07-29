using Container_App.Core.Model.KhachSans;
using Container_App.Service.Dtos.KhachSan;
using Container_App.Service.Dtos.KhachSanDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSans
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
