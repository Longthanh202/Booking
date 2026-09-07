using Booking.Core.Model.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.TienIchs
{
    public interface ITienIchRepository
    {
        Task<TienIch> ThemTienIch(TienIch tienIch);
        Task<List<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId);
    }
}
