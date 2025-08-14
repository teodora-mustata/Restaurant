using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public partial class EditProductsWindow : Window
        {
            public EditProductsWindow()
            {
                InitializeComponent();
                DataContext = new EditProductsViewModel();
            }

    }
}
 
