using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Restaurant.Database;
using Restaurant.Model;

namespace Restaurant.ViewModel
{
    public class EditIngredientsViewModel : INotifyPropertyChanged
    {
        private readonly IngredientService ingredientService;
        public ObservableCollection<Ingredient> Ingredients { get; set; }

        private Ingredient _selectedIngredient;
        public Ingredient SelectedIngredient
        {
            get => _selectedIngredient;
            set
            {
                if (_selectedIngredient != value)
                {
                    _selectedIngredient = value;
                    IngredientName = _selectedIngredient?.Name ?? string.Empty;
                    OnPropertyChanged(nameof(SelectedIngredient));
                }
            }
        }

        private string _ingredientName;
        public string IngredientName
        {
            get => _ingredientName;
            set
            {
                if (_ingredientName != value)
                {
                    _ingredientName = value;
                    OnPropertyChanged(nameof(IngredientName));
                }
            }
        }

        public ICommand AddIngredientCommand { get; }
        public ICommand UpdateIngredientCommand { get; }
        public ICommand DeleteIngredientCommand { get; }

        public EditIngredientsViewModel()
        {
            ingredientService = new IngredientService();
            Ingredients = new ObservableCollection<Ingredient>(ingredientService.GetAllIngredients());

            AddIngredientCommand = new RelayCommand(AddIngredient);
            UpdateIngredientCommand = new RelayCommand(UpdateIngredient);
            DeleteIngredientCommand = new RelayCommand(DeleteIngredient);
        }

        private void AddIngredient()
        {
            if (!string.IsNullOrWhiteSpace(IngredientName))
            {
                ingredientService.AddIngredient(IngredientName);
                Refresh();
                IngredientName = string.Empty;
            }
            else
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void UpdateIngredient()
        {
            if (SelectedIngredient != null && !string.IsNullOrWhiteSpace(IngredientName))
            {
                ingredientService.UpdateIngredient(SelectedIngredient.IngredientID, IngredientName);
                Refresh();
                IngredientName = string.Empty;
            }
            else
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void DeleteIngredient()
        {
            if (SelectedIngredient != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete ingredient \"{SelectedIngredient.Name}\" and all associated data?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    ingredientService.DeleteIngredient(SelectedIngredient.IngredientID);
                    Refresh();
                    IngredientName = string.Empty;
                }
            }
        }


        private void Refresh()
        {
            Ingredients.Clear();
            foreach (var ing in ingredientService.GetAllIngredients())
                Ingredients.Add(ing);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
