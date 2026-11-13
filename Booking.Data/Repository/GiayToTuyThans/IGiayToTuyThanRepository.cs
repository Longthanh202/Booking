using Booking.Core.Model.GiayToTuyThans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.GiayToTuyThans
{
    public interface IGiayToTuyThanRepository
    {
        // Đọc
        Task<GiayToTuyThan?> GetByIdAsync(long id, bool track = false, CancellationToken ct = default);
        Task<List<GiayToTuyThan>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
        Task<GiayToTuyThan?> GetGiayToChinhAsync(Guid userProfileId, CancellationToken ct = default);
        Task<bool> ExistsByHashAsync(byte loaiGiayTo, byte[] hash, long? boQuaId = null, CancellationToken ct = default);
        Task<bool> HasGiayToChinhAsync(Guid userProfileId, CancellationToken ct = default);

        // Ghi (mỗi method là một đơn vị nguyên tử)
        Task<GiayToTuyThan> AddAsync(GiayToTuyThan entity, CancellationToken ct = default);
        Task UpdateAsync(GiayToTuyThan entity, CancellationToken ct = default);
        Task SetGiayToChinhAsync(GiayToTuyThan entity, CancellationToken ct = default);
        Task SoftDeleteAsync(GiayToTuyThan entity, string nguoiXoa, CancellationToken ct = default);
    }
}
