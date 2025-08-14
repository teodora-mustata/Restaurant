using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Database;
using Restaurant;
using Restaurant.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Printing;
using System.Windows;

namespace Restaurant.ViewModel
{
    public class ShowMenuViewModel : INotifyPropertyChanged
    {
        public bool IsClientLoggedIn => Session.Role == "client";

        private readonly ProductService _productService = new();
        private readonly IngredientService _ingredientService = new();
        private readonly AllergenService _allergenService = new();
        private readonly CategoryService _categoryService = new();
        private readonly BundleService _bundleService = new();

        public ObservableCollection<Product> AllProducts { get; set; }
        public ObservableCollection<Ingredient> AllIngredients { get; set; }
        public ObservableCollection<Allergen> AllAllergens { get; set; }
        public ObservableCollection<Category> AllCategories { get; set; }
        public ObservableCollection<Bundle> AllBundles { get; set; }

        public ObservableCollection<object> FilteredItems { get; set; } = new();
        public List<object> AllItems { get; set; } = new();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    ApplyFilter();
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilter();
            }
        }

        private bool _excludeMatches;
        public bool ExcludeMatches
        {
            get => _excludeMatches;
            set
            {
                _excludeMatches = value;
                OnPropertyChanged(nameof(ExcludeMatches));
                ApplyFilter();
            }
        }


        private void ApplyFilter()
        {

            FilteredItems.Clear();

            string keyword = SearchText?.Trim().ToLower() ?? "";

            foreach (var product in AllProducts)
            {
                bool matches = string.IsNullOrEmpty(keyword) ||
                               product.Name.ToLower().Contains(keyword) ||
                               product.Ingredients.Any(i => i.Name.ToLower().Contains(keyword)) ||
                               product.Allergens.Any(a => a.Name.ToLower().Contains(keyword));

                if (ExcludeMatches ? !matches : matches)
                {
                    if (SelectedCategory == null || SelectedCategory.Name == "All" ||
                        (product.Category?.CategoryID == SelectedCategory.CategoryID))
                    {
                        FilteredItems.Add(product);
                    }
                }
            }

            foreach (var bundle in AllBundles)
            {
                bool matches = string.IsNullOrEmpty(keyword) ||
                               bundle.Name.ToLower().Contains(keyword) ||
                               bundle.BundleProducts.Any(bp => bp.Product.Name.ToLower().Contains(keyword));

                if (ExcludeMatches ? !matches : matches)
                {
                    if (SelectedCategory == null || SelectedCategory.Name == "All" ||
                        bundle.BundleProducts.Any(bp =>
                            bp.Product?.Category?.CategoryID == SelectedCategory.CategoryID))
                    {
                        FilteredItems.Add(bundle);
                    }
                }
            }
        }

        public ObservableCollection<OrderItem> Cart { get; set; } = new();

        private int _selectedQuantity = 1;
        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                _selectedQuantity = value;
                OnPropertyChanged(nameof(SelectedQuantity));
                (AddToCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private object _selectedMenuItem;
        public object SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged(nameof(SelectedMenuItem));
                (AddToCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddToCartCommand => new RelayCommand(AddToCart);

        private bool CanAddToCart()
        {
            if (SelectedMenuItem == null || SelectedQuantity <= 0)
                return false;

            if (SelectedMenuItem is Product p)
                return !p.IsOutOfStock;

            if (SelectedMenuItem is Bundle b)
                return !b.IsOutOfStock;

            return false;
        }
        //private void AddToCart()
        //{
        //    if (!CanAddToCart()) return;

        //    //var existing = Cart.FirstOrDefault(ci =>
        //    //    (ci.Product != null && ci.Product == SelectedMenuItem) ||
        //    //    (ci.Bundle != null && ci.Bundle == SelectedMenuItem));

        //    //if (existing != null)
        //    //{
        //    //    existing.Quantity += SelectedQuantity;
        //    //}
        //    //else
        //    //{
        //    //    Cart.Add(new OrderItem
        //    //    {
        //    //        Product = SelectedMenuItem as Product,
        //    //        Bundle = SelectedMenuItem as Bundle,
        //    //        Quantity = SelectedQuantity
        //    //    });
        //    //}

        //    Cart.Add(new OrderItem
        //    {
        //        Product = SelectedMenuItem as Product,
        //        Bundle = SelectedMenuItem as Bundle,
        //        Quantity = SelectedQuantity
        //    });

        //    OnPropertyChanged(nameof(Cart));
        //    OnPropertyChanged(nameof(CartTotalPrice));
        //}
        private void AddToCart()
        {
            if (!CanAddToCart()) return;

            Product selectedProduct = SelectedMenuItem as Product;
            Bundle selectedBundle = SelectedMenuItem as Bundle;

            int totalRequested = SelectedQuantity;

            if (selectedProduct != null)
            {
                int totalQuantityAlreadyInCart = Cart
                    .Where(ci => ci.Product != null && ci.Product.ProductID == selectedProduct.ProductID)
                    .Sum(ci => ci.Quantity);

                int totalRequiredQuantity = (totalQuantityAlreadyInCart + totalRequested) * selectedProduct.QuantityPerServing;

                if (!selectedProduct.HasEnoughStock(totalRequiredQuantity))
                {
                    MessageBox.Show($"Not enough quantity for {selectedProduct.Name} in stock.", "Not enough stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (selectedBundle != null)
            {
                foreach (var bp in selectedBundle.BundleProducts)
                {
                    int productId = bp.Product.ProductID;
                    int bundleQuantity = bp.Quantity * SelectedQuantity;

                    int quantityAlreadyInCartFromBundles = Cart
                        .Where(ci => ci.Bundle != null)
                        .SelectMany(ci => ci.Bundle.BundleProducts)
                        .Where(bp2 => bp2.Product.ProductID == productId)
                        .Sum(bp2 => bp2.Quantity);

                    int totalRequired = bundleQuantity + quantityAlreadyInCartFromBundles;

                    if (!bp.Product.HasEnoughStock(totalRequired))
                    {
                        MessageBox.Show($"Not enough quantity for {bp.Product.Name} in stock to add to bundle {selectedBundle.Name}.", "Not enough stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            Cart.Add(new OrderItem
            {
                Product = selectedProduct,
                Bundle = selectedBundle,
                Quantity = SelectedQuantity
            });

            OnPropertyChanged(nameof(Cart));
            OnPropertyChanged(nameof(CartTotalPrice));
        }


        public decimal CartTotalPrice => Cart.Sum(item => item.TotalPrice);

        private OrderItem _selectedCartItem;
        public OrderItem SelectedCartItem
        {
            get => _selectedCartItem;
            set
            {
                _selectedCartItem = value;
                OnPropertyChanged(nameof(SelectedCartItem));
                (RemoveFromCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        public ICommand RemoveFromCartCommand => new RelayCommand(RemoveFromCart, CanRemoveFromCart);

        private bool CanRemoveFromCart()
        {
            return SelectedCartItem != null;
        }

        private void RemoveFromCart()
        {
            if (SelectedCartItem != null)
            {
                Cart.Remove(SelectedCartItem);
                SelectedCartItem = null;

                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(CartTotalPrice));
            }
        }


        public ShowMenuViewModel()
        {
            AllProducts = new ObservableCollection<Product>(_productService.GetAllProducts());
            AllIngredients = new ObservableCollection<Ingredient>(_ingredientService.GetAllIngredients());
            AllAllergens = new ObservableCollection<Allergen>(_allergenService.GetAllAllergens());
            AllCategories = new ObservableCollection<Category>(_categoryService.GetAllCategories());
            AllBundles = new ObservableCollection<Bundle>(_bundleService.GetAllBundles());

            AllCategories.Insert(0, new Category { CategoryID = -1, Name = "All" });
            SelectedCategory = AllCategories.First();
            ApplyFilter();
            Session.LoggedUserChanged += OnLoggedUserChanged;
        }

        private void OnLoggedUserChanged(string propertyName)
        {
            if (propertyName == nameof(Session.Role))
            {
                OnPropertyChanged(nameof(IsClientLoggedIn));
            }
        }

        public ICommand PlaceOrderCommand => new RelayCommand(PlaceOrder, CanPlaceOrder);

        private bool CanPlaceOrder()
        {
            return Cart.Any() && Session.Role == "client";
        }


        private void PlaceOrder()
        {
            try
            {
                var orderService = new OrderService();
                var discountManager = new OrderDiscountManager();

                int clientId = orderService.GetClientIdByEmail(Session.Email);
                var previousOrders = orderService.GetOrdersByClientId(clientId);

                var order = new Order
                {
                    ClientID = clientId,
                    DatePlaced = DateTime.Now,
                    EstimatedDeliveryTime = DateTime.Now.AddHours(2),
                    Status = OrderStatus.Registered,
                    Items = new ObservableCollection<OrderItem>(Cart)
                };

                //discountManager.ProcessOrder(order, previousOrders);

                orderService.PlaceOrder(order);

                Cart.Clear();
                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(CartTotalPrice));
                MessageBox.Show($"Order successfully placed! \nFinal price: {order.TotalPrice:F2} lei", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while placing order: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
