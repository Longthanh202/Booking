using System.Security.Claims;

namespace Booking.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? UserProfileId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id) ? id : null;
        }
        public static string? TenNguoiDung(this ClaimsPrincipal user)
        {
            return user.FindFirst("FullName")?.Value;
        }

        public static bool LaQuanTri(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        public static int? RoleId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("RoleId")?.Value;

            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
