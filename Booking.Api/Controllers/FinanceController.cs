using Booking.Service.Dtos.Finance;
using Booking.Service.Services.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/finance")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _service;

        public FinanceController(IFinanceService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("wallets")]
        public async Task<IActionResult> GetWallets()
        {
            return !Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId)
                ? Unauthorized()
                : Ok(await _service.GetWallets(ownerId));
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("statement")]
        public async Task<IActionResult> GetStatement(Guid? khachSanId, DateTime? from, DateTime? to)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            return Ok(await _service.GetStatement(ownerId, khachSanId, from, to));
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("bank-accounts")]
        public async Task<IActionResult> GetBankAccounts()
        {
            return !Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId)
                ? Unauthorized()
                : Ok(await _service.GetBankAccounts(ownerId));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("bank-accounts")]
        public async Task<IActionResult> AddBankAccount([FromBody] CreateBankAccountRequest request)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var account = await _service.AddBankAccount(ownerId, request);
            return account == null ? BadRequest(new { message = "Thông tin tài khoản ngân hàng không hợp lệ." }) : Ok(account);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("withdrawals")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] CreateWithdrawalRequest request)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var payout = await _service.RequestWithdrawal(ownerId, request);
            return payout == null ? BadRequest(new { message = "Số dư, tài khoản ngân hàng hoặc property không hợp lệ." }) : Ok(payout);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("withdrawals")]
        public async Task<IActionResult> GetWithdrawals()
        {
            return !Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId)
                ? Unauthorized()
                : Ok(await _service.GetWithdrawals(ownerId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/withdrawals/pending")]
        public async Task<IActionResult> GetPendingWithdrawals() => Ok(await _service.GetPendingWithdrawals());

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/withdrawals/{id:long}")]
        public async Task<IActionResult> UpdateWithdrawal(long id, [FromBody] UpdatePayoutRequest request)
        {
            var payout = await _service.UpdateWithdrawal(id, request);
            return payout == null ? BadRequest(new { message = "Yêu cầu rút tiền hoặc trạng thái không hợp lệ." }) : Ok(payout);
        }
    }
}