using System.Security.Claims;
using Container_App.Data.Repository.Users;
using Container_App.Service.Services.LichSuVis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LichSuViController : Controller
{
    private readonly ILichSuViService _lichSuViService;
    private readonly IUserServices  _userServices;
    public LichSuViController(ILichSuViService lichSuViService,
        IUserServices  userServices)
    {
        _lichSuViService = lichSuViService;
        _userServices = userServices;
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("owner")]
    public async Task<IActionResult> LayLichSuViOwner()
    {
        if(!_userServices.IsAuthenticated())
        {
            return Unauthorized(new { Message = "Vui lòng đăng nhập để xem lịch sử hoa hong" });
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("OwnerId không hợp lệ.");
        }

        var result = await _lichSuViService.LayLichSuViOwner(Guid.Parse(userId));

        return Ok(result);
    }
}