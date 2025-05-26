using UserManagement.Core.Models.Auth;
using UserManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Web.Models;

namespace UserManagement.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            AuthRequest authRequest = new AuthRequest
            {
                Email = loginModel.Email,
                Password = loginModel.Password
            };

            AuthResult result = await _authService.LoginAsync(authRequest);

            if (!result.Success)
            {
                TempData[WebConstants.TempData.ERROR_MESSAGE] = result.Message;
                return RedirectToAction(WebConstants.Actions.LOGIN);
            }

            int expiryInDays = int.Parse(_configuration[WebConstants.Configuration.EXPIRE_IN_DAYS]);

            Response.Cookies.Append(WebConstants.Auth.JWT_COOKIE_NAME, result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(expiryInDays)
            });

            return RedirectToAction(WebConstants.Actions.INDEX, WebConstants.Controllers.USER);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var registerRequest = new RegisterRequest
            {
                Name = registerModel.Name,
                Email = registerModel.Email,
                Password = registerModel.Password
            };

            var result = await _authService.RegisterAsync(registerRequest);

            if (!result.Success)
            {
                TempData[WebConstants.TempData.ERROR_MESSAGE] = result.Message;
                return RedirectToAction(WebConstants.Actions.REGISTER);
            }

            TempData[WebConstants.TempData.SUCCESS_MESSAGE] = result.Message;
            return RedirectToAction(WebConstants.Actions.LOGIN);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(WebConstants.Auth.JWT_COOKIE_NAME);
            return RedirectToAction(WebConstants.Actions.LOGIN);
        }
    }


}
