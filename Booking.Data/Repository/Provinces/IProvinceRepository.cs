using Booking.Core.Model.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Provinces
{
    public interface IProvinceRepository
    {
        Task<List<Province>> GetProvinces();
    }
}
