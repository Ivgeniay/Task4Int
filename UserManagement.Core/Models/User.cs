namespace UserManagement.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public UserStatus Status { get; set; }


        public static User CreateRndUser()
        {
            string GetRndName()
            {
                string[] names = { "John", "Peter", "Anna", "Maria", "Alex", "Elena", "Sergey", "Natalie", "Michael", "Sarah", "David", "Emma", "James", "Lisa", "Robert", "Kate" };
                return names[Random.Shared.Next(names.Length)];
            }
            string GetRndPasswordHash() => Guid.NewGuid().ToString("N")[..8];
            string GetRndEmail()
            {
                string[] domains = { "gmail.com", "mail.ru", "yandex.ru", "hotmail.com" };
                return $"user{Random.Shared.Next(100, 9999)}@{domains[Random.Shared.Next(domains.Length)]}";
            }
            DateTime GetRandomDateTime() => DateTime.Now.AddDays(-Random.Shared.Next(1, 365));

            int rnd = Random.Shared.Next(0, 2);
            return new User()
            {
                Name = GetRndName(),
                Email = GetRndEmail(),
                PasswordHash = GetRndPasswordHash(),
                RegistrationDate = GetRandomDateTime(),
                LastLoginDate = GetRandomDateTime(),
                Status = rnd > 0 ? UserStatus.Active : UserStatus.Blocked 
            };
        }
    }

    public enum UserStatus
    {
        Active,
        Blocked
    }
}
