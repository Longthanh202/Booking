using Booking.Core.Model.GiayToTuyThans;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.GiayToTuyThans
{
    public class GiayToTuyThanRepository: IGiayToTuyThanRepository
    {
        private readonly AppDbContext _db;

        public GiayToTuyThanRepository(AppDbContext db) => _db = db;

        private IQueryable<GiayToTuyThan> Query => _db.Set<GiayToTuyThan>().Where(x => !x.IsDel);

        // ---------- Đọc ----------

        public Task<GiayToTuyThan?> GetByIdAsync(long id, bool track = false, CancellationToken ct = default)
        {
            var q = track ? Query : Query.AsNoTracking();
            return q.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<List<GiayToTuyThan>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
            => Query.AsNoTracking()
                    .Where(x => x.UserProfileId == userProfileId)
                    .OrderByDescending(x => x.LaGiayToChinh).ThenByDescending(x => x.NgayTao)
                    .ToListAsync(ct);

        public Task<GiayToTuyThan?> GetGiayToChinhAsync(Guid userProfileId, CancellationToken ct = default)
            => Query.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserProfileId == userProfileId && x.LaGiayToChinh, ct);

        public Task<bool> ExistsByHashAsync(byte loaiGiayTo, byte[] hash, long? boQuaId = null, CancellationToken ct = default)
            => Query.AnyAsync(x => x.LoaiGiayTo == loaiGiayTo
                                && x.SoGiayToHash == hash
                                && (boQuaId == null || x.Id != boQuaId), ct);

        public Task<bool> HasGiayToChinhAsync(Guid userProfileId, CancellationToken ct = default)
            => Query.AnyAsync(x => x.UserProfileId == userProfileId && x.LaGiayToChinh, ct);

        // ---------- Ghi ----------

        public async Task<GiayToTuyThan> AddAsync(GiayToTuyThan entity, CancellationToken ct = default)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tran = await _db.Database.BeginTransactionAsync(ct);

                // Unique index chỉ cho 1 giấy tờ chính/người: bỏ cờ cũ trước khi thêm dòng chính mới
                if (entity.LaGiayToChinh)
                    await BoCoChinhAsync(entity.UserProfileId, null, entity.NguoiTao, ct);

                _db.Set<GiayToTuyThan>().Add(entity);
                await _db.SaveChangesAsync(ct);
                await tran.CommitAsync(ct);
            });
            return entity;
        }

        public async Task UpdateAsync(GiayToTuyThan entity, CancellationToken ct = default)
        {
            // entity đã được track (lấy bằng GetByIdAsync(track: true))
            await _db.SaveChangesAsync(ct);
        }

        public async Task SetGiayToChinhAsync(GiayToTuyThan entity, CancellationToken ct = default)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tran = await _db.Database.BeginTransactionAsync(ct);

                await BoCoChinhAsync(entity.UserProfileId, entity.Id, entity.NguoiCapNhat, ct);

                entity.LaGiayToChinh = true;
                await _db.SaveChangesAsync(ct);
                await tran.CommitAsync(ct);
            });
        }

        public async Task SoftDeleteAsync(GiayToTuyThan entity, string nguoiXoa, CancellationToken ct = default)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tran = await _db.Database.BeginTransactionAsync(ct);

                var laChinh = entity.LaGiayToChinh;
                entity.IsDel = true;
                entity.LaGiayToChinh = false;
                entity.NgayCapNhat = DateTime.UtcNow;
                entity.NguoiCapNhat = nguoiXoa;
                await _db.SaveChangesAsync(ct);

                // Xóa giấy tờ chính thì chuyển cờ chính sang giấy tờ mới nhất còn lại
                if (laChinh)
                {
                    var ke = await Query.Where(x => x.UserProfileId == entity.UserProfileId)
                                        .OrderByDescending(x => x.NgayTao)
                                        .FirstOrDefaultAsync(ct);
                    if (ke is not null)
                    {
                        ke.LaGiayToChinh = true;
                        ke.NgayCapNhat = DateTime.UtcNow;
                        ke.NguoiCapNhat = nguoiXoa;
                        await _db.SaveChangesAsync(ct);
                    }
                }

                await tran.CommitAsync(ct);
            });
        }

        private async Task<int> BoCoChinhAsync(
            Guid userProfileId,
            long? boQuaId,
            string? nguoi,
            CancellationToken ct)
        {
            var entities = await Query
                .Where(x =>
                    x.UserProfileId == userProfileId &&
                    x.LaGiayToChinh &&
                    (boQuaId == null || x.Id != boQuaId))
                .ToListAsync(ct);

            foreach (var entity in entities)
            {
                entity.LaGiayToChinh = false;
                entity.NgayCapNhat = DateTime.UtcNow;
                entity.NguoiCapNhat = nguoi;
            }

            await _db.SaveChangesAsync(ct);

            return entities.Count;
        }
    }
}
