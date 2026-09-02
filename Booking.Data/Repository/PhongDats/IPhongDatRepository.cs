using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.PhongDats
{
    public interface IPhongDatRepository
    {
        Task<PhongDat> Tao(PhongDat phongDat);
    }
}
