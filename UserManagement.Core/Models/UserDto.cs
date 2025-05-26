namespace UserManagement.Core.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsCurrentUser { get; set; }
    }

}
