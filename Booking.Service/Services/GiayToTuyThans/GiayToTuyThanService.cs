using Booking.Core.Model.GiayToTuyThans;
using Booking.Data.Repository.GiayToTuyThans;
using Booking.Service.Dtos.GiayToTuyThans;
using Booking.Service.Services.Cryptos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.GiayToTuyThans
{
    public class GiayToTuyThanService : IGiayToTuyThanService
    {
        // Trạng thái xác minh: 0 chờ duyệt, 1 đã duyệt, 2 từ chối
        private const byte ChoDuyet = 0, DaDuyet = 1, TuChoi = 2;

        private readonly IGiayToTuyThanRepository _repo;
        private readonly ICryptoService _crypto;

        public GiayToTuyThanService(IGiayToTuyThanRepository repo, ICryptoService crypto)
        {
            _repo = repo;
            _crypto = crypto;
        }

        // ---------- Đọc ----------

        public async Task<GiayToTuyThanDto?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var e = await _repo.GetByIdAsync(id, false, ct);
            return e is null ? null : ToDto(e);
        }

        public async Task<List<GiayToTuyThanDto>> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            var list = await _repo.GetByUserProfileIdAsync(userProfileId, ct);
            return list.Select(ToDto).ToList();
        }

        public async Task<GiayToTuyThanDto?> GetGiayToChinhAsync(Guid userProfileId, CancellationToken ct = default)
        {
            var e = await _repo.GetGiayToChinhAsync(userProfileId, ct);
            return e is null ? null : ToDto(e);
        }

        public async Task<string?> GetSoGiayToRoAsync(long id, CancellationToken ct = default)
        {
            var e = await _repo.GetByIdAsync(id, false, ct);
            return e is null ? null : _crypto.Decrypt(e.SoGiayTo);
        }

        public Task<bool> TonTaiSoGiayToAsync(byte loaiGiayTo, string soGiayTo, long? boQuaId = null, CancellationToken ct = default)
            => _repo.ExistsByHashAsync(loaiGiayTo, _crypto.ComputeHash(Normalize(soGiayTo)), boQuaId, ct);

        // ---------- Thêm / sửa ----------

        public async Task<long> CreateAsync(GiayToTuyThanCreateDto dto, string nguoiTao, CancellationToken ct = default)
        {
            var so = Normalize(dto.SoGiayTo);
            if (string.IsNullOrEmpty(so))
                throw new ArgumentException("Số giấy tờ không được để trống.", nameof(dto));

            var hash = _crypto.ComputeHash(so);
            if (await _repo.ExistsByHashAsync(dto.LoaiGiayTo, hash, null, ct))
                throw new InvalidOperationException("Số giấy tờ này đã được sử dụng.");

            // Giấy tờ đầu tiên của người dùng tự thành giấy tờ chính
            var laChinh = dto.LaGiayToChinh || !await _repo.HasGiayToChinhAsync(dto.UserProfileId, ct);

            var entity = new GiayToTuyThan
            {
                UserProfileId = dto.UserProfileId,
                LoaiGiayTo = dto.LoaiGiayTo,
                SoGiayTo = _crypto.Encrypt(so),
                SoGiayToHash = hash,
                HoTenTrenGiayTo = dto.HoTenTrenGiayTo,
                NgayCap = dto.NgayCap,
                NgayHetHan = dto.NgayHetHan,
                NoiCap = dto.NoiCap,
                AnhMatTruoc = dto.AnhMatTruoc,
                AnhMatSau = dto.AnhMatSau,
                TrangThaiXacMinh = ChoDuyet,
                LaGiayToChinh = laChinh,
                NgayTao = DateTime.UtcNow,
                NguoiTao = nguoiTao
            };

            await _repo.AddAsync(entity, ct);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(long id, GiayToTuyThanUpdateDto dto, string nguoiCapNhat, CancellationToken ct = default)
        {
            var e = await _repo.GetByIdAsync(id, track: true, ct);
            if (e is null) return false;

            var so = Normalize(dto.SoGiayTo);
            if (string.IsNullOrEmpty(so))
                throw new ArgumentException("Số giấy tờ không được để trống.", nameof(dto));

            var hash = _crypto.ComputeHash(so);
            var doiSo = e.LoaiGiayTo != dto.LoaiGiayTo || !hash.AsSpan().SequenceEqual(e.SoGiayToHash);

            if (doiSo)
            {
                if (await _repo.ExistsByHashAsync(dto.LoaiGiayTo, hash, id, ct))
                    throw new InvalidOperationException("Số giấy tờ này đã được sử dụng.");

                e.LoaiGiayTo = dto.LoaiGiayTo;
                e.SoGiayTo = _crypto.Encrypt(so);
                e.SoGiayToHash = hash;
            }

            var doiAnh = e.AnhMatTruoc != dto.AnhMatTruoc || e.AnhMatSau != dto.AnhMatSau;

            e.HoTenTrenGiayTo = dto.HoTenTrenGiayTo;
            e.NgayCap = dto.NgayCap;
            e.NgayHetHan = dto.NgayHetHan;
            e.NoiCap = dto.NoiCap;
            e.AnhMatTruoc = dto.AnhMatTruoc;
            e.AnhMatSau = dto.AnhMatSau;

            // Đổi số hoặc ảnh thì phải duyệt lại
            if (doiSo || doiAnh)
            {
                e.TrangThaiXacMinh = ChoDuyet;
                e.NguoiXacMinh = null;
                e.NgayXacMinh = null;
                e.GhiChuXacMinh = null;
            }

            e.NgayCapNhat = DateTime.UtcNow;
            e.NguoiCapNhat = nguoiCapNhat;

            await _repo.UpdateAsync(e, ct);
            return true;
        }

        // ---------- Xác minh ----------

        public Task<bool> DuyetAsync(long id, string nguoiXacMinh, CancellationToken ct = default)
            => DoiTrangThaiAsync(id, DaDuyet, nguoiXacMinh, null, ct);

        public Task<bool> TuChoiAsync(long id, string nguoiXacMinh, string lyDo, CancellationToken ct = default)
            => DoiTrangThaiAsync(id, TuChoi, nguoiXacMinh, lyDo, ct);

        private async Task<bool> DoiTrangThaiAsync(long id, byte trangThai, string nguoi, string? ghiChu, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(id, track: true, ct);
            if (e is null) return false;

            e.TrangThaiXacMinh = trangThai;
            e.NguoiXacMinh = nguoi;
            e.NgayXacMinh = DateTime.UtcNow;
            e.GhiChuXacMinh = ghiChu;
            e.NgayCapNhat = DateTime.UtcNow;
            e.NguoiCapNhat = nguoi;

            await _repo.UpdateAsync(e, ct);
            return true;
        }

        // ---------- Giấy tờ chính / xóa ----------

        public async Task<bool> DatLamGiayToChinhAsync(long id, string nguoiCapNhat, CancellationToken ct = default)
        {
            var e = await _repo.GetByIdAsync(id, track: true, ct);
            if (e is null) return false;
            if (e.LaGiayToChinh) return true;

            e.NgayCapNhat = DateTime.UtcNow;
            e.NguoiCapNhat = nguoiCapNhat;
            await _repo.SetGiayToChinhAsync(e, ct);
            return true;
        }

        public async Task<bool> XoaAsync(long id, string nguoiXoa, CancellationToken ct = default)
        {
            var e = await _repo.GetByIdAsync(id, track: true, ct);
            if (e is null) return false;

            await _repo.SoftDeleteAsync(e, nguoiXoa, ct);
            return true;
        }

        // ---------- Helper ----------

        private static string Normalize(string? s) => (s ?? string.Empty).Trim().ToUpperInvariant();

        private GiayToTuyThanDto ToDto(GiayToTuyThan e) => new()
        {
            Id = e.Id,
            UserProfileId = e.UserProfileId,
            LoaiGiayTo = e.LoaiGiayTo,
            SoGiayToDaChe = Che(_crypto.Decrypt(e.SoGiayTo)),
            HoTenTrenGiayTo = e.HoTenTrenGiayTo,
            NgayCap = e.NgayCap,
            NgayHetHan = e.NgayHetHan,
            NoiCap = e.NoiCap,
            AnhMatTruoc = e.AnhMatTruoc,
            AnhMatSau = e.AnhMatSau,
            TrangThaiXacMinh = e.TrangThaiXacMinh,
            NguoiXacMinh = e.NguoiXacMinh,
            NgayXacMinh = e.NgayXacMinh,
            GhiChuXacMinh = e.GhiChuXacMinh,
            LaGiayToChinh = e.LaGiayToChinh,
            NgayTao = e.NgayTao
        };

        private static string Che(string s)
            => s.Length <= 4 ? new string('*', s.Length) : new string('*', s.Length - 4) + s[^4..];
    }
}
