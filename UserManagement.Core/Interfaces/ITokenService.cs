using UserManagement.Core.Models;

namespace UserManagement.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        int? ValidateJwtToken(string token);
    }
}
