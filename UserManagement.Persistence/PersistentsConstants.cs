namespace UserManagement.Persistence
{
    public class PersistentsConstants
    {
        public const string USER_ID = "User_id";
        public const string PK_USER_INDEX = "PK_Users_id";
        public const string EMAIL_INDEX_NAME = "IX_Users_Email";
        public const int PASSWORD_HASH_LENGTH = 256;
        public const int EMAIL_LENGTH = 100;
        public const int NAME_LENGTH = 50;
    }
}
