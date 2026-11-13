using Booking.Api.Extensions;
using Booking.Service.Dtos.GiayToTuyThans;
using Booking.Service.Services.GiayToTuyThans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

public record TuChoiGiayToDto([property: Required, StringLength(500, MinimumLength = 3)] string LyDo);

namespace Booking.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/giay-to-tuy-than")]
    public class GiayToTuyThanController : ControllerBase
    {
        private readonly IGiayToTuyThanService _service;
        private readonly ILogger<GiayToTuyThanController> _logger;

        public GiayToTuyThanController(IGiayToTuyThanService service, ILogger<GiayToTuyThanController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ---------- Người dùng thao tác trên giấy tờ của mình ----------

        [HttpGet("cua-toi")]
        public async Task<ActionResult<List<GiayToTuyThanDto>>> CuaToi(CancellationToken ct)
        {
            var profileId = User.UserProfileId();
            if (profileId is null) return Forbid();

            return Ok(await _service.GetByUserProfileIdAsync(profileId.Value, ct));
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<GiayToTuyThanDto>> Get(long id, CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(id, ct);
            if (dto is null || !DuocTruyCap(dto.UserProfileId)) return NotFound();   // 404 để không lộ giấy tờ có tồn tại

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GiayToTuyThanDto>> Tao([FromBody] GiayToTuyThanCreateDto dto, CancellationToken ct)
        {
            var profileId = User.UserProfileId();
            if (profileId is null) return Forbid();

            dto.UserProfileId = profileId.Value;   // bắt buộc ghi đè: không tin UserProfileId client gửi lên

            var id = await _service.CreateAsync(dto, User.TenNguoiDung(), ct);
            var created = await _service.GetByIdAsync(id, ct);
            return CreatedAtAction(nameof(Get), new { id }, created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> CapNhat(long id, [FromBody] GiayToTuyThanUpdateDto dto, CancellationToken ct)
        {
            if (!await LaChuSoHuuAsync(id, ct)) return NotFound();

            var ok = await _service.UpdateAsync(id, dto, User.TenNguoiDung(), ct);
            return ok ? NoContent() : NotFound();
        }

        [HttpPut("{id:long}/giay-to-chinh")]
        public async Task<IActionResult> DatLamChinh(long id, CancellationToken ct)
        {
            if (!await LaChuSoHuuAsync(id, ct)) return NotFound();

            var ok = await _service.DatLamGiayToChinhAsync(id, User.TenNguoiDung(), ct);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Xoa(long id, CancellationToken ct)
        {
            if (!await LaChuSoHuuAsync(id, ct)) return NotFound();

            var ok = await _service.XoaAsync(id, User.TenNguoiDung(), ct);
            return ok ? NoContent() : NotFound();
        }

        // ---------- Chỉ Admin / NguoiDuyet ----------

        [HttpGet("nguoi-dung/{userProfileId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<GiayToTuyThanDto>>> TheoNguoiDung(Guid userProfileId, CancellationToken ct)
            => Ok(await _service.GetByUserProfileIdAsync(userProfileId, ct));

        [HttpPost("{id:long}/duyet")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duyet(long id, CancellationToken ct)
        {
            var ok = await _service.DuyetAsync(id, User.TenNguoiDung(), ct);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("{id:long}/tu-choi")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TuChoi(long id, [FromBody] TuChoiGiayToDto dto, CancellationToken ct)
        {
            var ok = await _service.TuChoiAsync(id, User.TenNguoiDung(), dto.LyDo, ct);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>Xem số giấy tờ đầy đủ: hạn chế vai trò, luôn ghi audit, không cache.</summary>
        [HttpGet("{id:long}/so-giay-to")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XemSoGiayTo(long id, CancellationToken ct)
        {
            var so = await _service.GetSoGiayToRoAsync(id, ct);
            if (so is null) return NotFound();

            // Chỉ ghi Id và người xem, KHÔNG ghi số giấy tờ
            _logger.LogWarning("AUDIT: {User} đã xem số giấy tờ đầy đủ của GiayToTuyThan Id={Id}", User.TenNguoiDung(), id);

            Response.Headers.CacheControl = "no-store";
            return Ok(new { soGiayTo = so });
        }

        [HttpGet("kiem-tra-ton-tai")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> KiemTraTonTai([FromQuery] byte loaiGiayTo, [FromQuery] string soGiayTo, CancellationToken ct)
            => Ok(new { tonTai = await _service.TonTaiSoGiayToAsync(loaiGiayTo, soGiayTo, null, ct) });

        // ---------- Helper ----------

        private bool DuocTruyCap(Guid userProfileId)
            => User.LaQuanTri() || User.UserProfileId() == userProfileId;

        private async Task<bool> LaChuSoHuuAsync(long id, CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(id, ct);
            return dto is not null && dto.UserProfileId == User.UserProfileId();
        }
    }
}
