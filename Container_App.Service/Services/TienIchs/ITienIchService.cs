using Container_App.Core.Model.TienIchs;
using Container_App.Service.Dtos.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.TienIchs
{
    public interface ITienIchService
    {
        Task<TienIch> ThemTienIch(TienIchRequest tienIch);
        Task<List<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId);
    }
}
