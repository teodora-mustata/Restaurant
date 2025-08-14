using System;
using System.Globalization;
using System.Windows.Data;
using Restaurant.Database;
using Restaurant.Model;

namespace Restaurant
{
    public class UserFromIdConverter : IValueConverter
    {
        private readonly UserService _userService = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int clientId)
            {
                try
                {
                    User user = _userService.GetUserById(clientId);
                    return $"{user.FirstName} {user.LastName}\n{user.Email}\n{user.PhoneNumber}";
                }
                catch
                {
                    return "Unknown User";
                }
            }

            return "Invalid ID";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
