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
using Restaurant.ViewModel;

namespace Restaurant.View
{
    /// <summary>
    /// Interaction logic for EditAllergensWindow.xaml
    /// </summary>
    public partial class EditAllergensWindow : Window
    {
        public EditAllergensWindow()
        {
            InitializeComponent();
            DataContext = new EditAllergensViewModel();
        }
    }
}
