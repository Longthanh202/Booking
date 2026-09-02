using Booking.Data.Repository.Permissions;
using Booking.Service.Services.Permissions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Principal;

namespace Booking.Api.Attributes
{
    public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _resource;
        private readonly string _action;
        

        public HasPermissionAttribute(string resource, string action )
        {
            _resource = resource;
            _action = action;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            var user = context.HttpContext.User;

           
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleId = user.FindFirst("RoleId")?.Value;
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(roleId))
            {
                context.Result = new ForbidResult();
                return;
            }

            var services = context.HttpContext.RequestServices;
            var cache = services.GetService<IMemoryCache>();
            var permissionService = services.GetService<IPermissionService>();
            var cacheKey = $"Permission_Role_{roleId}";

            if (!cache.TryGetValue(cacheKey, out List<string> permissions))
            {
                var permissionsFromDb = (await permissionService
                .GetListPermissionByUser(Guid.Parse(userId)))
                .ToList();

                permissions = permissionsFromDb
                    .Select(p => $"{p.ResourceName.ToLower()}_{p.Action.ToLower()}")
                    .ToList();

                cache.Set(cacheKey, permissions, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
            }

            var requiredPermission = $"{_resource}_{_action}";

            if (!permissions.Contains(requiredPermission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
