using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.HoaHongs
{
    public interface IHoaHongService
    {
        Task TinhHoaHong(Guid datPhongId);
    }
}
