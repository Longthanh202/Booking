using Booking.Core.Model.LichSuVis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.LichSuVis
{
    public interface ILichSuViRepository
    {
        Task<LichSuVi?> LayTheoId(long id);

        Task<IEnumerable<LichSuVi>> LayTheoVi(long viKhachSanId);

        Task<IEnumerable<LichSuVi>> LayTheoDatPhong(Guid datPhongId);
        
        Task<IEnumerable<LichSuVi>> LayTheoOwnerId(Guid ownerId);

        Task<IEnumerable<LichSuVi>> LayTheoKhoangThoiGian(
            long viKhachSanId,
            DateTime tuNgay,
            DateTime denNgay);

        Task Tao(LichSuVi lichSuVi);
    }
}
