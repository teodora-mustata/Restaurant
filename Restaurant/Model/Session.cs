using System;

namespace Restaurant.Model
{
    public static class Session
    {
        private static string _firstName;
        private static string _role;
        private static string _email;

        public static event Action<string> LoggedUserChanged;

        public static string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnLoggedUserChanged(nameof(FirstName));
                }
            }
        }

        public static string Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnLoggedUserChanged(nameof(Role));
                }
            }
        }

        public static string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnLoggedUserChanged(nameof(Email));
                }
            }
        }

        private static void OnLoggedUserChanged(string propertyName)
        {
            LoggedUserChanged?.Invoke(propertyName);
        }
    }
}
