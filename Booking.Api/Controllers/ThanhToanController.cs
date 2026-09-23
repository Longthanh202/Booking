using Booking.Service.Dtos.Payments;
using Booking.Service.Services.ThanhToans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IThanhToanService _service;

        public ThanhToanController(IThanhToanService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomerPayment(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId))
            {
                return Unauthorized();
            }

            var payment = await _service.GetForCustomer(id, customerId);
            return payment == null ? NotFound() : Ok(payment);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllPayments() => Ok(await _service.GetAll());

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusRequest request)
        {
            var payment = await _service.UpdateStatus(id, request);
            return payment == null ? BadRequest(new { message = "Payment không tồn tại hoặc trạng thái không hợp lệ." }) : Ok(payment);
        }
    }
}