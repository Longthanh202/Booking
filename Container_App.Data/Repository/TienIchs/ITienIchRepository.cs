using Container_App.Core.Model.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.TienIchs
{
    public interface ITienIchRepository
    {
        Task<TienIch> ThemTienIch(TienIch tienIch);
        Task<List<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId);
    }
}
