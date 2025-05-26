using System.IdentityModel.Tokens.Jwt;
using UserManagement.Core;
using UserManagement.Core.Interfaces;

namespace UserManagement.Web.Middleware
{
    internal class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserStatusMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var path = context.Request.Path.Value?.ToLower();

                if (IsAuthPath(path))
                {
                    await _next(context);
                    return;
                }

                var userIdClaim = context.User.FindFirst(Constants.Auth.USER_ID_CLAIM);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                        var isBlockedOrDeleted = await userService.IsUserBlockedOrDeletedAsync(userId);
                        if (isBlockedOrDeleted)
                        {
                            context.Response.Redirect(WebConstants.Routes.AUTH_LOGIN);
                            return;
                        }
                    }
                }

                await _next(context);
            }
            catch
            {

            }
        }

        private bool IsAuthPath(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.Contains(WebConstants.Routes.AUTH_LOGIN) ||
                   path.Contains(WebConstants.Routes.AUTH_REGISTER) ||
                   path.Contains(WebConstants.Routes.HOME) ||
                   path.StartsWith(WebConstants.Routes.CSS_PATH) ||
                   path.StartsWith(WebConstants.Routes.JS_PATH) ||
                   path.StartsWith(WebConstants.Routes.LIB_PATH);
        }
    }

}
