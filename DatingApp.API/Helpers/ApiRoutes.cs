namespace DatingApp.API.Helpers
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        public const string DefaultController = Root + "/[controller]";

        public static class Auth
        {
            public const string Register = "register";
            public const string Login = "login";
        }

        public static class Messages
        {
            public const string Controller = Root + "/users/{userId}/[controller]";
            public const string GetMessage = "{id}";
            public const string GetMessageThread = "thread/{recipientId}";
            public const string DeleteMessage = "{id}";
            public const string MarkMessageAsRead = "{id}/read";
        }

        public static class Users
        {
            public const string GetUser = "{id}";
            public const string UpdateUser = "{id}";
            public const string LikeUser = "{id}/like/{recipientId}";
        }

        public static class Photos
        {
            public const string Controller = Root + "/users/{userId}/photos";
            public const string GetPhoto = "{id}";
            public const string SetMainPhoto = "{id}/setMain";
            public const string DeletePhoto = "{id}";
        }
    }
}
