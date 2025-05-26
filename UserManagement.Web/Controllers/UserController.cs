using Microsoft.AspNetCore.Mvc;
using UserManagement.Core;
using UserManagement.Core.Interfaces;
using UserManagement.Core.Models;

namespace UserManagement.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction(WebConstants.Actions.LOGIN, WebConstants.Controllers.AUTH);

            var users = await _userService.GetAllUsersAsync();
            var currentUser = users.FirstOrDefault(u => u.Id != currentUserId.Value);
            if (currentUser != null) currentUser.IsCurrentUser = true;

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Block([FromBody] int[] userIds)
        {
            await _userService.BlockUsersAsync(userIds);

            var currentUserId = GetCurrentUserId();
            if (currentUserId.HasValue && userIds.Contains(currentUserId.Value))
            {
                Response.Cookies.Delete(WebConstants.Auth.JWT_COOKIE_NAME);
                return Json(new ApiResponse
                {
                    Success = true,
                    RedirectToLogin = true,
                    Message = "You have blocked yourself"
                });
            }

            return Json(new ApiResponse
            {
                Success = true,
                Message = "Users blocked"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Unblock([FromBody] int[] userIds)
        {
            await _userService.UnblockUsersAsync(userIds);
            return Json(new ApiResponse
            {
                Success = true,
                Message = "Users unblocked"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int[] userIds)
        {
            await _userService.DeleteUsersAsync(userIds);

            var currentUserId = GetCurrentUserId();
            if (currentUserId.HasValue && userIds.Contains(currentUserId.Value))
            {
                Response.Cookies.Delete(WebConstants.Auth.JWT_COOKIE_NAME);
                return Json(new ApiResponse
                {
                    Success = true,
                    RedirectToLogin = true,
                    Message = "You have deleted yourself"
                });
            }

            return Json(new ApiResponse
            {
                Success = true,
                Message = "Users deleted"
            });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst(Constants.Auth.USER_ID_CLAIM);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
    }

}
