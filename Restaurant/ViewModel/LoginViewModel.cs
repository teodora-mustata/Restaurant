using System.Windows;
using System.Windows.Input;
using Restaurant.Model;

namespace Restaurant.ViewModel
{
    public class LoginViewModel
    {
        private UserService userService = new UserService();

        public string Email { get; set; }
        public string Password { get; set; }

        public string Role {  get; set; }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<Window>(Login);
        }

        private void Login(Window window)
        {
            var (role, firstName) = userService.LoginUser(Email, Password);

            if (role != null)
            {
                System.Windows.MessageBox.Show($"Login successful! Welcome, {role} {firstName}!");

                window?.Close(); // Inchide fereastra de login
                                 // aici poti trimite informatia spre MainWindow ca sa stie cine s-a logat
                Session.Email = Email;
                Session.Role = role;
                Session.FirstName = firstName;
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid credentials!");
            }
        }

    }
}
