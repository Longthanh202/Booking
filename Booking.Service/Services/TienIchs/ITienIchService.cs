using Booking.Core.Model.TienIchs;
using Booking.Service.Dtos.Amenities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.TienIchs
{
    public interface ITienIchService
    {
        Task<int> CreateAmenities(List<CreateAmenityRequest> tienIch);
        Task<List<TienIch>> GetAmenitiesByHotelId(Guid khachSanId);
    }
}
