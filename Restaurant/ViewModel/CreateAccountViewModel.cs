using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace Restaurant.ViewModel
{
    public class CreateAccountViewModel
    {
        private UserService userService = new UserService();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public ICommand CreateAccountCommand { get; }

        public CreateAccountViewModel()
        {
            CreateAccountCommand = new RelayCommand<Window>(CreateAccount);
        }

        private void CreateAccount(Window window)
        {
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||
                string.IsNullOrWhiteSpace(Address))
            {
                System.Windows.MessageBox.Show("Please fill in all fields!");
                return;
            }

            try
            {
                userService.AddUser(FirstName, LastName, Email, Password, PhoneNumber, Address, "client");
                System.Windows.MessageBox.Show("Account created successfully!");
                window?.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error creating account: {ex.Message}");
                window?.Close();
            }
        }

    }
}
