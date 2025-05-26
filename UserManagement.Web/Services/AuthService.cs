using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserManagement.Core.Interfaces;
using UserManagement.Core.Models;
using UserManagement.Core.Models.Auth;
using UserManagement.Persistence;

namespace UserManagement.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> LoginAsync(AuthRequest loginRequest)
        {
            var user = await _userService.GetUserByEmailAsync(loginRequest.Email);

            if (user == null)
                return new AuthResult { Success = false, Message = "Invalid email or password" };

            if (user.Status == UserStatus.Blocked)
                return new AuthResult { Success = false, Message = "User is blocked" };

            var passwordHash = HashPassword(loginRequest.Password);
            if (passwordHash != user.PasswordHash)
                return new AuthResult { Success = false, Message = "Invalid email or password" };

            await _userService.UpdateLastLoginTimeAsync(user.Id);

            var token = _tokenService.GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = user,
                Message = "Success"
            };
        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                var passwordHash = HashPassword(registerRequest.Password);

                var newUser = new User
                {
                    Name = registerRequest.Name,
                    Email = registerRequest.Email,
                    PasswordHash = passwordHash,
                    RegistrationDate = DateTime.UtcNow,
                    Status = UserStatus.Active
                };

                await _userService.AddUserAsync(newUser);

                return new RegisterResult { Success = true, Message = "Success registration" };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains(PersistentsConstants.EMAIL_INDEX_NAME) == true)
            {
                return new RegisterResult { Success = false, Message = "User with this email already exist" };
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, Message = "Registration failed. Please try again." };
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
