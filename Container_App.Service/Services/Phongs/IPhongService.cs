using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Phongs
{
    public interface IPhongService
    {
        Task<int> TaoPhong(Phong p);
    }
}
