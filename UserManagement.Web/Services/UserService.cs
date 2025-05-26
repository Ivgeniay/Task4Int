using UserManagement.Core.Interfaces;
using UserManagement.Core.Models;
using UserManagement.Persistence.Interfaces;

namespace UserManagement.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users
                .OrderByDescending(u => u.LastLoginDate ?? u.RegistrationDate)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    LastLoginDate = u.LastLoginDate,
                    Status = u.Status.ToString(),
                    IsCurrentUser = false
                });
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task UpdateLastLoginTimeAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task BlockUsersAsync(IEnumerable<int> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.Status = UserStatus.Blocked;
                    await _userRepository.UpdateAsync(user);
                }
            }
        }

        public async Task UnblockUsersAsync(IEnumerable<int> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.Status = UserStatus.Active;
                    await _userRepository.UpdateAsync(user);
                }
            }
        }

        public async Task DeleteUsersAsync(IEnumerable<int> userIds)
        {
            foreach (var userId in userIds)
            {
                await _userRepository.DeleteAsync(userId);
            }
        }

        public async Task<bool> IsUserBlockedOrDeletedAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null || user.Status == UserStatus.Blocked;
        }
    }
}
