using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Restaurant.Model;
using Restaurant.Database;

namespace Restaurant.ViewModel
{
    public class EditCategoryViewModel : INotifyPropertyChanged
    {
        private readonly CategoryService categoryService;
        public ObservableCollection<Category> Categories { get; set; }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    CategoryName = _selectedCategory?.Name ?? string.Empty;
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value;
                    OnPropertyChanged(nameof(CategoryName));
                }
            }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand UpdateCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public EditCategoryViewModel()
        {
            categoryService = new CategoryService();
            Categories = new ObservableCollection<Category>(categoryService.GetAllCategories());

            AddCategoryCommand = new RelayCommand(AddCategory);
            UpdateCategoryCommand = new RelayCommand(UpdateCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
        }

        private void AddCategory()
        {
            if (!string.IsNullOrWhiteSpace(CategoryName))
            {
                categoryService.AddCategory(CategoryName);
                Refresh();
                CategoryName = string.Empty;
            }
            else
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void UpdateCategory()
        {
            if (SelectedCategory != null && !string.IsNullOrWhiteSpace(CategoryName))
            {
                categoryService.UpdateCategory(SelectedCategory.CategoryID, CategoryName);
                Refresh();
                CategoryName = string.Empty;
            }
            else
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void DeleteCategory()
        {
            if (SelectedCategory != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete category \"{SelectedCategory.Name}\" and all associated data?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    categoryService.DeleteCategory(SelectedCategory.CategoryID);
                    Refresh();
                    CategoryName = string.Empty;
                }
            }
        }


        private void Refresh()
        {
            Categories.Clear();
            foreach (var c in categoryService.GetAllCategories())
                Categories.Add(c);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
