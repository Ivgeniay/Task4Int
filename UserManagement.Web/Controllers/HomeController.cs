using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UserManagement.Core;
using UserManagement.Core.Interfaces;
using UserManagement.Web.Models;

namespace UserManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId.HasValue)
            {
                var isBlockedOrDeleted = await _userService.IsUserBlockedOrDeletedAsync(currentUserId.Value);
                if (!isBlockedOrDeleted)
                {
                    return RedirectToAction(WebConstants.Actions.INDEX, WebConstants.Controllers.USER);
                }
            }

            return RedirectToAction(WebConstants.Actions.LOGIN, WebConstants.Controllers.AUTH);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst(Constants.Auth.USER_ID_CLAIM);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : null;
        }
    }
}
