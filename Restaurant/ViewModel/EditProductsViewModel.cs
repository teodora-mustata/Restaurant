using Microsoft.Win32;
using Restaurant.Database;
using Restaurant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Restaurant.ViewModel
{
    public class EditProductsViewModel : INotifyPropertyChanged
    {
        private readonly ProductService _productService = new();
        private readonly IngredientService _ingredientService = new();
        private readonly AllergenService _allergenService = new();
        private readonly CategoryService _categoryService = new();

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Ingredient> AllIngredients { get; set; }
        public ObservableCollection<Allergen> AllAllergens { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if (_selectedProduct != null)
                {
                    EditableProduct = CloneProduct(_selectedProduct);
                    //EditableProduct.Ingredients = new ObservableCollection<Ingredient>(_productService.GetProductIngredients(_selectedProduct.ProductID));
                    //EditableProduct.Allergens = new ObservableCollection<Allergen>(_productService.GetProductAllergens(_selectedProduct.ProductID));
                    OnPropertyChanged(nameof(EditableProduct));
                }
                OnPropertyChanged(nameof(SelectedProduct));
                OnPropertyChanged(nameof(SelectedProduct.Ingredients));
                OnPropertyChanged(nameof(SelectedProduct.Allergens));
            }
        }

        private Product _editableProduct;
        public Product EditableProduct
        {
            get => _editableProduct;
            set
            {
                _editableProduct = value;
                OnPropertyChanged(nameof(EditableProduct));
                OnPropertyChanged(nameof(EditableProduct.Ingredients));
                OnPropertyChanged(nameof(EditableProduct.Allergens));
            }
        }

        private Product CloneProduct(Product original)
        {
            if (original == null)
                return null;

            var ingredients = _productService.GetProductIngredients(original.ProductID);
            var allergens = _productService.GetProductAllergens(original.ProductID);

            var clone = new Product
            {
                ProductID = original.ProductID,
                Name = original.Name,
                Price = original.Price,
                QuantityPerServing = original.QuantityPerServing,
                TotalQuantity = original.TotalQuantity,
                IsVegan = original.IsVegan,
                IsVegetarian = original.IsVegetarian,
                Category = original.Category,
                ImagePath = original.ImagePath,
                Ingredients = new ObservableCollection<Ingredient>(ingredients),
                Allergens = new ObservableCollection<Allergen>(allergens)
            };
            return clone;
        }


        public ICommand SaveProductCommand => new RelayCommand(SaveProduct);
        public ICommand DeleteProductCommand => new RelayCommand(DeleteProduct);
        public ICommand AddProductCommand => new RelayCommand(AddProduct);
        public ICommand SelectImageCommand => new RelayCommand(SelectImage);

        public EditProductsViewModel()
        {
            Products = new ObservableCollection<Product>(_productService.GetAllProducts());
            AllIngredients = new ObservableCollection<Ingredient>(_ingredientService.GetAllIngredients());
            AllAllergens = new ObservableCollection<Allergen>(_allergenService.GetAllAllergens());
            Categories = new ObservableCollection<Category>(_categoryService.GetAllCategories());

            EditableProduct = new Product
            {
                Ingredients = new ObservableCollection<Ingredient>(),
                Allergens = new ObservableCollection<Allergen>()
            };
            SelectedProduct = null;
        }

        private void SaveProduct()
        {
            if (EditableProduct == null ||
                string.IsNullOrWhiteSpace(EditableProduct.Name) ||
                EditableProduct.Price <= 0 ||
                EditableProduct.QuantityPerServing <= 0 ||
                EditableProduct.TotalQuantity < 0 ||
                EditableProduct.Category == null ||
                EditableProduct.Category.CategoryID == 0 ||
                string.IsNullOrWhiteSpace(EditableProduct.ImagePath))
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EditableProduct.ProductID != 0)
            {
                foreach (var i in EditableProduct.Ingredients)
                    Debug.WriteLine($"{i.IngredientID} - {i.Name}");
                foreach (var i in EditableProduct.Allergens)
                    Debug.WriteLine($"{i.AllergenID} - {i.Name}");

                _productService.UpdateProduct(EditableProduct);
                _productService.UpdateProductIngredients(EditableProduct.ProductID, EditableProduct.Ingredients);
                _productService.UpdateProductAllergens(EditableProduct.ProductID, EditableProduct.Allergens);

                Products = new ObservableCollection<Product>(_productService.GetAllProducts());
                OnPropertyChanged(nameof(Products));

                SelectedProduct = null;
                SelectedProduct = Products.FirstOrDefault(p => p.ProductID == EditableProduct.ProductID);
                EditableProduct = CloneProduct(SelectedProduct);
            }
        }

        private void DeleteProduct()
        {
            if (EditableProduct == null || EditableProduct.ProductID == 0)
                return;

            _productService.DeleteProduct(EditableProduct.ProductID);
            Products.Remove(Products.FirstOrDefault(p => p.ProductID == EditableProduct.ProductID));

            EditableProduct.Ingredients.Clear();
            EditableProduct.Allergens.Clear();

        }

        private void AddProduct()
        {
            Debug.WriteLine("Nume: " + EditableProduct.Name);
            Debug.WriteLine("Pret: " + EditableProduct.Price);
            Debug.WriteLine("Categoria: " + EditableProduct.Category?.Name);
            Debug.WriteLine("Image: " + EditableProduct.ImagePath);
            if (EditableProduct == null ||
                string.IsNullOrWhiteSpace(EditableProduct.Name) ||
                EditableProduct.Price <= 0 ||
                EditableProduct.QuantityPerServing <= 0 ||
                EditableProduct.TotalQuantity < 0 ||
                EditableProduct.Category == null ||
                EditableProduct.Category.CategoryID == 0 ||
                string.IsNullOrWhiteSpace(EditableProduct.ImagePath))
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newProductId = _productService.AddProduct(EditableProduct);

            foreach (var ingredient in EditableProduct.Ingredients)
                _productService.AddIngredientToProduct(newProductId, ingredient.IngredientID);

            foreach (var allergen in EditableProduct.Allergens)
                _productService.AddAllergenToProduct(newProductId, allergen.AllergenID);

            Products = new ObservableCollection<Product>(_productService.GetAllProducts());
            OnPropertyChanged(nameof(Products));

            EditableProduct.Ingredients.Clear();
            EditableProduct.Allergens.Clear();

        }

        private void SelectImage()
        {
            if (EditableProduct == null)
                EditableProduct = new Product();

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg;*.jpeg;*.png",
                InitialDirectory = Path.GetFullPath(@"..\..\..\Images")
            };

            if (dialog.ShowDialog() == true)
            {
                EditableProduct.ImagePath = dialog.FileName;
                OnPropertyChanged(nameof(EditableProduct.Category));
                OnPropertyChanged(nameof(EditableProduct));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
