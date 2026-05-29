using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.WPF
{
    public static class Session
    {
        public static User CurrentUser { get; private set; }

        public static void Open(User user)
        {
            CurrentUser = user;
        }

        public static void Close()
        {
            CurrentUser = null;
        }

        public static bool IsLoggedIn => CurrentUser != null;
    }
}
