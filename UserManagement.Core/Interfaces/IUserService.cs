using UserManagement.Core.Models;

namespace UserManagement.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AddUserAsync(User user);
        Task UpdateLastLoginTimeAsync(int userId);
        Task BlockUsersAsync(IEnumerable<int> userIds);
        Task UnblockUsersAsync(IEnumerable<int> userIds);
        Task DeleteUsersAsync(IEnumerable<int> userIds);
        Task<bool> IsUserBlockedOrDeletedAsync(int userId);
    }
}
