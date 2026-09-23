using Booking.Service.Dtos.Advertising;
using Booking.Service.Services.Advertising;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/advertising")]
    [ApiController]
    public class AdvertisingController : ControllerBase
    {
        private readonly IAdvertisingService _service;

        public AdvertisingController(IAdvertisingService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("packages")]
        public async Task<IActionResult> GetPackages() => Ok(await _service.GetPackages());

        [Authorize(Roles = "Owner")]
        [HttpGet("owner")]
        public async Task<IActionResult> GetOwnerCampaigns()
        {
            return !Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId)
                ? Unauthorized()
                : Ok(await _service.GetOwnerCampaigns(ownerId));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateAdvertisingRequest request)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var order = await _service.CreateOrder(ownerId, request);
            return order == null ? BadRequest(new { message = "Property hoặc gói quảng cáo không hợp lệ." }) : Ok(order);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("orders/{id:long}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(long id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var order = await _service.ConfirmPayment(ownerId, id);
            return order == null ? BadRequest(new { message = "Đơn quảng cáo không ở trạng thái chờ thanh toán." }) : Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/pending")]
        public async Task<IActionResult> GetPendingApproval() => Ok(await _service.GetPendingApproval());

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id:long}/approve")]
        public async Task<IActionResult> Approve(long id, [FromBody] ApproveAdvertisingRequest request)
        {
            var campaign = await _service.Approve(id, request);
            return campaign == null ? BadRequest(new { message = "Chiến dịch không hợp lệ hoặc đã được xử lý." }) : Ok(campaign);
        }
    }
}