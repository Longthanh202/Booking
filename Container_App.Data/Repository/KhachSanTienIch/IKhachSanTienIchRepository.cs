using Container_App.Core.Model.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSanTienIch
{
    public interface IKhachSanTienIchRepository
    {
        Task<KhachSan_TienIch> Tao(KhachSan_TienIch entity);
    }
}
