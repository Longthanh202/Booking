using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.PhongDats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.PhongDats
{
    public interface IPhongDatRepository
    {
        Task<PhongDat> Tao(PhongDat phongDat);
    }
}
