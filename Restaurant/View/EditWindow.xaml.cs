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

namespace Restaurant.View
{

    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            InitializeComponent();
        }

        private void EditIngredientsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new EditIngredientsWindow();
            window.Closed += (s, args) => this.Show(); 
            window.Show();
        }


        private void EditAllergensButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new EditAllergensWindow();
            window.Closed += (s, args) => this.Show();
            window.Show();
        }

        private void EditCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new EditCategoriesWindow();
            window.Closed += (s, args) => this.Show();
            window.Show();
        }

        private void EditProductsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new EditProductsWindow();
            window.Closed += (s, args) => this.Show();
            window.Show();
        }

        private void EditBundlesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new EditBundlesWindow();
            window.Closed += (s, args) => this.Show();
            window.Show();
        }
    }
}
