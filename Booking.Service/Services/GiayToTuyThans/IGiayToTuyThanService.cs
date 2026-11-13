using Booking.Service.Dtos.GiayToTuyThans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.GiayToTuyThans
{
    public interface IGiayToTuyThanService
    {
        Task<GiayToTuyThanDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<List<GiayToTuyThanDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
        Task<GiayToTuyThanDto?> GetGiayToChinhAsync(Guid userProfileId, CancellationToken ct = default);

        /// <summary>Giải mã số giấy tờ. Chỉ gọi ở nơi đã kiểm tra quyền.</summary>
        Task<string?> GetSoGiayToRoAsync(long id, CancellationToken ct = default);

        Task<bool> TonTaiSoGiayToAsync(byte loaiGiayTo, string soGiayTo, long? boQuaId = null, CancellationToken ct = default);

        Task<long> CreateAsync(GiayToTuyThanCreateDto dto, string nguoiTao, CancellationToken ct = default);
        Task<bool> UpdateAsync(long id, GiayToTuyThanUpdateDto dto, string nguoiCapNhat, CancellationToken ct = default);

        Task<bool> DuyetAsync(long id, string nguoiXacMinh, CancellationToken ct = default);
        Task<bool> TuChoiAsync(long id, string nguoiXacMinh, string lyDo, CancellationToken ct = default);

        Task<bool> DatLamGiayToChinhAsync(long id, string nguoiCapNhat, CancellationToken ct = default);
        Task<bool> XoaAsync(long id, string nguoiXoa, CancellationToken ct = default);
    }
}
