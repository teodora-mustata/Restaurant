using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Database;
using Restaurant.Model;
using Restaurant;
using System.ComponentModel;

namespace Restaurant.ViewModel
{
    public class LowStockProductsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Product> LowStockProducts { get; set; }

        private readonly ProductService _productService = new();
        private readonly CategoryService _categoryService = new();

        public ObservableCollection<Product> AllProducts { get; set; }
        public ObservableCollection<Category> AllCategories { get; set; }

        private double _lowStockMultiplier;
        public double LowStockMultiplier
        {
            get => _lowStockMultiplier;
            set
            {
                _lowStockMultiplier = value;
                OnPropertyChanged(nameof(LowStockMultiplier));
                OnPropertyChanged(nameof(LowStockMultiplierDisplay));
            }
        }

        public string LowStockMultiplierDisplay => $"{LowStockMultiplier}x";

        public LowStockProductsViewModel()
        {
            AllProducts = new ObservableCollection<Product>(_productService.GetAllProducts());
            AllCategories = new ObservableCollection<Category>(_categoryService.GetAllCategories());

            LowStockMultiplier = int.Parse(AppConfig.LowStockMultiplier);
            LowStockProducts = new ObservableCollection<Product>( AllProducts.Where(p => p.TotalQuantity <= LowStockMultiplier * p.QuantityPerServing));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
