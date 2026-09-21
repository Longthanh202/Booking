using Booking.Core.Model.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ThanhToans
{
    public interface IThanhToanRepository
    {
        Task<ThanhToan> CreatePayment(ThanhToan t);
        Task<ThanhToan?> LayTheoDatPhong(Guid datPhongId);
    }
}
