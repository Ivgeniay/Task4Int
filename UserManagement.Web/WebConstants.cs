namespace UserManagement.Web
{
    public static class WebConstants
    {
        public static class Auth
        {
            public const string AUTHORIZATION_HEADER = "Authorization";
            public const string BEARER_PREFIX = "Bearer ";
            public const string JWT_COOKIE_NAME = "jwt";
        }

        public static class Routes
        {
            public const string AUTH_LOGIN = "/auth/login";
            public const string AUTH_LOGOUT = "/auth/logout";
            public const string AUTH_REGISTER = "/auth/register";
            public const string HOME = "/home";
            public const string CSS_PATH = "/css";
            public const string JS_PATH = "/js";
            public const string LIB_PATH = "/lib";
        }

        public static class Controllers
        {
            public const string AUTH = "Auth";
            public const string USER = "User";
        }

        public static class Configuration
        {
            public const string CONNECTION_STR = "DefaultConnection";
            public const string SECRET ="Jwt:Secret";
            public const string ISSUER = "Jwt:Issuer";
            public const string AUDIENCE = "Jwt:Audience";
            public const string EXPIRE_IN_DAYS = "Jwt:ExpiryInDays";
        }

        public static class Actions
        {
            public const string LOGIN = "Login";
            public const string REGISTER = "Register";
            public const string INDEX = "Index";
        }

        public static class TempData
        {
            public const string SUCCESS_MESSAGE = "SuccessMessage";
            public const string ERROR_MESSAGE = "ErrorMessage";
        }
    }
}
