using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container_App.Core.Model.HoaHongs;
using Container_App.Service.Dtos.HoaHong;

namespace Container_App.Service.Services.HoaHongs
{
    public interface IHoaHongService
    {
        Task TinhHoaHong(Guid datPhongId);
        Task<List<HoaHongDto>> LayTheoOwnerId(Guid ownerId);
    }
}
