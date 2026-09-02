using Booking.Core.Model.DatPhongs;
using Booking.Service.Dtos.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.DatPhongs
{
    public interface IDatPhongService
    {
        Task<DatPhong> DatPhong(DatPhongRequest dp, Guid userId);
        Task<DatPhong> CapNhatTrangThai(Guid id);
        Task<DatPhongOwnerDto> GetListBookingOwner(DatPhongOwnerRequest dto, Guid ownerId);
        Task<BookingHistory> BookingHistory(Guid userId, int pageIndex, int pageSize);
        Task CheckIn(Guid id);
        Task XacNhan(Guid id);
    }
}
