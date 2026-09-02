using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Core.Model.HoaHongs;
using Booking.Service.Dtos.HoaHong;

namespace Booking.Service.Services.HoaHongs
{
    public interface IHoaHongService
    {
        Task TinhHoaHong(Guid datPhongId);
        Task<List<HoaHongDto>> LayTheoOwnerId(Guid ownerId);
    }
}
