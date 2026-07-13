using Container_App.Core.Model.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Interface.TienIchs
{
    public interface ITienIchService
    {
        Task<int> ThemTienIch(TienIch tienIch);
        Task<IEnumerable<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId);
    }
}
