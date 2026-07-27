using Container_App.Core.Model.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.ThanhToans
{
    public interface IThanhToanRepository
    {
        Task<ThanhToan> Tao(ThanhToan t);
        Task<ThanhToan?> LayTheoDatPhong(Guid datPhongId);
    }
}
