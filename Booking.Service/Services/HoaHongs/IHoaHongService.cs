using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Core.Model.HoaHongs;
using Booking.Service.Dtos.Commissions;

namespace Booking.Service.Services.HoaHongs
{
    public interface IHoaHongService
    {
        Task CalculateCommission(Guid datPhongId);
        Task<List<CommissionDto>> GetCommissionsByOwnerId(Guid ownerId);
    }
}
