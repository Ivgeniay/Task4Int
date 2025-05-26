using UserManagement.Core.Models.Auth;

namespace UserManagement.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(AuthRequest loginRequest);
        Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
    }
}
