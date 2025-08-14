using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using Restaurant.Model;
using System.IO;
using Restaurant.Database;
using System.Windows.Input;
using System.Globalization;
using System.Windows;

namespace Restaurant.ViewModel
{
    public class EditBundlesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ProductService _productService = new();
        private readonly CategoryService _categoryService = new();
        private readonly BundleService _bundleService = new();

        private Bundle _selectedBundle;
        public Bundle SelectedBundle
        {
            get => _selectedBundle;
            set
            {
                _selectedBundle = value;
                OnPropertyChanged(nameof(SelectedBundle));
                if (value != null)
                {
                    EditableBundle = CloneBundle(value);
                    OnPropertyChanged(nameof(EditableBundle));
                    OnPropertyChanged(nameof(EditableBundleDisplayPrice));
                }

            }
        }

        private Bundle _editableBundle;
        public Bundle EditableBundle
        {
            get => _editableBundle;
            set
            {
                _editableBundle = value;

                if (_editableBundle != null)
                {
                    _editableBundle.BundleProducts.CollectionChanged += (_, __) =>
                    {
                        OnPropertyChanged(nameof(EditableBundleDisplayPrice));
                    };
                }

                OnPropertyChanged(nameof(EditableBundle));
                OnPropertyChanged(nameof(EditableBundleDisplayPrice));
            }
        }


        public ObservableCollection<Bundle> AllBundles { get; set; }
        public ObservableCollection<Product> AllProducts { get; set; }
        public ObservableCollection<Category> AllCategories { get; set; }


        private double _discount;
        public double Discount
        {
            get => _discount;
            set
            {
                _discount = value;
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(DiscountDisplay));
            }
        }

        public string DiscountDisplay => $"{Discount * 100}%";

        public string EditableBundleDisplayPrice =>$"{EditableBundle.Price:0.00} lei";


        public Product SelectedProduct { get; set; }
        public int SelectedQuantity { get; set; }

        private BundleProduct _selectedBundleProduct;
        public BundleProduct SelectedBundleProduct
        {
            get => _selectedBundleProduct;
            set
            {
                _selectedBundleProduct = value;
                OnPropertyChanged(nameof(SelectedBundleProduct));
            }
        }


        public EditBundlesViewModel()
        {
            AllBundles = new ObservableCollection<Bundle>(_bundleService.GetAllBundles());
            AllProducts = new ObservableCollection<Product>(_productService.GetAllProducts());
            AllCategories = new ObservableCollection<Category>(_categoryService.GetAllCategories());

            Discount = double.Parse(AppConfig.BundleDiscount, CultureInfo.InvariantCulture);

            EditableBundle = new Bundle();
            SelectedBundle = null;
        }
        private Bundle CloneBundle(Bundle original)
        {
            var clone = new Bundle
            {
                BundleID = original.BundleID,
                Name = original.Name,
                ImagePath = original.ImagePath,
                Category = original.Category,
                BundleProducts = new ObservableCollection<BundleProduct>()
            };

            if (original.BundleProducts != null)
            {
                foreach (var bp in original.BundleProducts)
                {
                    clone.BundleProducts.Add(new BundleProduct
                    {
                        BundleProductID = bp.BundleProductID,
                        BundleID = bp.BundleID,
                        ProductID = bp.ProductID,
                        Quantity = bp.Quantity,
                        Product = bp.Product
                    });
                }
            }

            return clone;
        }


        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ICommand SaveBundleCommand => new RelayCommand(SaveBundle);
        public ICommand DeleteBundleCommand => new RelayCommand(DeleteBundle);
        public ICommand AddBundleCommand => new RelayCommand(AddBundle);
        public ICommand SelectImageCommand => new RelayCommand(SelectImage);

        public ICommand AddProductToBundleCommand => new RelayCommand(AddProductToBundle);

        public ICommand DeleteProductFromBundleCommand => new RelayCommand(DeleteProductFromBundle);
        //private void AddProductToBundle()
        //{
        //    if (EditableBundle == null ||
        //        string.IsNullOrWhiteSpace(EditableBundle.Name) ||
        //        EditableBundle.Category == null ||
        //        EditableBundle.Category.CategoryID == 0 ||
        //        string.IsNullOrWhiteSpace(EditableBundle.ImagePath) ||
        //        SelectedProduct == null ||
        //        SelectedQuantity <=0)
        //    {
        //        MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return;
        //    }

        //    else
        //    {
        //        var bundleProduct = new BundleProduct
        //        {
        //            Product = SelectedProduct,
        //            ProductID = SelectedProduct.ProductID,
        //            Quantity = SelectedQuantity,
        //            Bundle = EditableBundle,
        //            BundleID = EditableBundle.BundleID
        //        };
        //        EditableBundle.BundleProducts.Add(bundleProduct);
        //    }
        //    OnPropertyChanged(nameof(EditableBundleDisplayPrice));
        //}

        private void AddProductToBundle()
        {
            if (SelectedProduct != null && SelectedQuantity > 0)
            {
                var bundleProduct = new BundleProduct
                {
                    Product = SelectedProduct,
                    ProductID = SelectedProduct.ProductID,
                    Quantity = SelectedQuantity,
                    Bundle = EditableBundle,
                    BundleID = EditableBundle.BundleID
                };
                EditableBundle.BundleProducts.Add(bundleProduct);
            }
            OnPropertyChanged(nameof(EditableBundleDisplayPrice));
        }

        private void DeleteProductFromBundle()
        {
            if (EditableBundle != null && SelectedBundleProduct != null)
            {
                EditableBundle.BundleProducts.Remove(SelectedBundleProduct);
                OnPropertyChanged(nameof(EditableBundleDisplayPrice));
            }
        }

        public void SaveBundle()
        {
            if (EditableBundle == null )
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditableBundle.BundleID = SelectedBundle.BundleID;
            _bundleService.UpdateBundle(SelectedBundle.BundleID, EditableBundle);

            _bundleService.DeleteBundleProducts(SelectedBundle.BundleID);

            foreach (var bp in EditableBundle.BundleProducts)
            {
                bp.BundleID = SelectedBundle.BundleID;
                _bundleService.AddBundleProduct(bp);
            }

            var index = AllBundles.IndexOf(SelectedBundle);
            if (index >= 0)
                AllBundles[index] = CloneBundle(EditableBundle);

            SelectedBundle = null;
            EditableBundle = new Bundle();
        }


        public void DeleteBundle()
        {
            if (SelectedBundle == null) return;

            _bundleService.DeleteBundle(SelectedBundle.BundleID);
            AllBundles.Remove(SelectedBundle);

            SelectedBundle = null;
            EditableBundle = new Bundle();
        }


        public void AddBundle()
        {
            if (EditableBundle == null ||
                string.IsNullOrWhiteSpace(EditableBundle.Name) ||
                EditableBundle.Category == null ||
                EditableBundle.Category.CategoryID == 0 ||
                string.IsNullOrWhiteSpace(EditableBundle.ImagePath) ||
                SelectedProduct == null ||
                SelectedQuantity <= 0)
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newBundleId = _bundleService.AddBundleAndReturnId(EditableBundle);
            EditableBundle.BundleID = newBundleId;

            foreach (var bp in EditableBundle.BundleProducts)
            {
                bp.BundleID = newBundleId;
                _bundleService.AddBundleProduct(bp);
            }

            AllBundles.Add(CloneBundle(EditableBundle));
            EditableBundle = new Bundle();
            OnPropertyChanged(nameof(AllBundles));
        }


        private void SelectImage()
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg;*.jpeg;*.png",
                InitialDirectory = Path.GetFullPath(@"..\..\..\Images")
            };

            if (dialog.ShowDialog() == true)
            {
                EditableBundle.ImagePath = dialog.FileName;
                OnPropertyChanged(nameof(EditableBundle));
            }
        }
    }
}
