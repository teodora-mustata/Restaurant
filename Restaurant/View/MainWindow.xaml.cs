using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Restaurant.Model;
using Restaurant.ViewModel;

namespace Restaurant.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            Session.LoggedUserChanged += Session_LoggedUserChanged;

            UpdateRoleButtons();
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow createAccountWindow = new CreateAccountWindow();
            createAccountWindow.ShowDialog();
        }

        private void LoginButton_Click(Object sender, RoutedEventArgs e) 
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }


        private void ShowMenuButton_Click(Object sender, RoutedEventArgs e)
        {
            ShowMenuWindow showMenu = new ShowMenuWindow();
            showMenu.ShowDialog();
        }

        private void Session_LoggedUserChanged(string propertyName)
        {
            if (propertyName == nameof(Session.Role))
            {
                UpdateRoleButtons();
            }
        }

        private void UpdateRoleButtons()
        {
            RoleButtonsPanel.Children.Clear();

            if (Session.Role == "client")
            {
                var showClientOrderHistory = new Button
                {
                    Content = "Show Order History",
                    Width = 200,
                    Height = 50,
                    Background = Brushes.White
                };
                showClientOrderHistory.Click += ShowClientOrderHistor_Click;
                RoleButtonsPanel.Children.Add(showClientOrderHistory);

            }
            else if (Session.Role == "employee")
            {
                var editMenuButton = new Button
                {
                    Content = "Edit Menu",
                    Width = 200,
                    Height = 50,
                    Background = Brushes.White
                };
                editMenuButton.Click += EditMenuButton_Click;
                RoleButtonsPanel.Children.Add(editMenuButton);


                //RoleButtonsPanel.Children.Add(new Button { Content = "Show All Orders", Width = 200, Height = 50, Background = Brushes.White });
                var showEmployeeOrderHistory = new Button
                {
                    Content = "Show Order History",
                    Width = 200,
                    Height = 50,
                    Background = Brushes.White
                };
                showEmployeeOrderHistory.Click += ShowEmployeeOrderHistory_Click;
                RoleButtonsPanel.Children.Add(showEmployeeOrderHistory);


                var lowStockProductsButton = new Button
                {
                    Content = "Low Stock Products",
                    Width = 200,
                    Height = 50,
                    Background = Brushes.White
                };
                lowStockProductsButton.Click += LowStockProductsButton_Click;
                RoleButtonsPanel.Children.Add(lowStockProductsButton);

            }
        }
        private void EditMenuButton_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow();
            editWindow.ShowDialog();
        } 

        private void LowStockProductsButton_Click(object sender, RoutedEventArgs e)
        {
           LowStockProductsWindow lowStockProductsWindow = new LowStockProductsWindow();
            lowStockProductsWindow.ShowDialog();
        }

        private void ShowClientOrderHistor_Click(object sender, RoutedEventArgs e)
        {
            ShowClientOrderHistoryWindow showClientOrderHistoryWindow = new ShowClientOrderHistoryWindow();
            showClientOrderHistoryWindow.ShowDialog();
        } 
        private void ShowEmployeeOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            ShowEmployeeOrderHistory showEmployeeOrderHistoryWindow = new ShowEmployeeOrderHistory();
            showEmployeeOrderHistoryWindow.ShowDialog();
        }

    }
}
