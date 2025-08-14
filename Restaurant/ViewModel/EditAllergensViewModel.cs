using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Restaurant.Database;
using Restaurant.Model;

namespace Restaurant.ViewModel
{
    public class EditAllergensViewModel : INotifyPropertyChanged
    {
        private readonly AllergenService allergenService;
        public ObservableCollection<Allergen> Allergens { get; set; }

        private Allergen _selectedAllergen;
        public Allergen SelectedAllergen
        {
            get => _selectedAllergen;
            set
            {
                if (_selectedAllergen != value)
                {
                    _selectedAllergen = value;
                    AllergenName = _selectedAllergen?.Name ?? string.Empty;
                    OnPropertyChanged(nameof(SelectedAllergen));
                }
            }
        }

        private string _allergenName;
        public string AllergenName
        {
            get => _allergenName;
            set
            {
                if (_allergenName != value)
                {
                    _allergenName = value;
                    OnPropertyChanged(nameof(AllergenName));
                }
            }
        }

        public ICommand AddAllergenCommand { get; }
        public ICommand UpdateAllergenCommand { get; }
        public ICommand DeleteAllergenCommand { get; }
        public EditAllergensViewModel()
        {
            allergenService = new AllergenService();
            Allergens = new ObservableCollection<Allergen>(allergenService.GetAllAllergens());

            AddAllergenCommand = new RelayCommand(AddAllergen);
            UpdateAllergenCommand = new RelayCommand(UpdateAllergen);
            DeleteAllergenCommand = new RelayCommand(DeleteAllergen);
        }

        private void AddAllergen()
        {
            if (!string.IsNullOrWhiteSpace(AllergenName))
            {
                allergenService.AddAllergen(AllergenName);
                Refresh();
                AllergenName = string.Empty;
            }
            else
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void UpdateAllergen()
        {
            if (SelectedAllergen != null && !string.IsNullOrWhiteSpace(AllergenName))
            {
                allergenService.UpdateAllergen(SelectedAllergen.AllergenID, AllergenName);
                Refresh();
                AllergenName = string.Empty;
            }
            else
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void DeleteAllergen()
        {
            if (SelectedAllergen != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete allergen \"{SelectedAllergen.Name}\" and all associated data?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    allergenService.DeleteAllergen(SelectedAllergen.AllergenID);
                    Refresh();
                    AllergenName = string.Empty;
                }
            }
        }


        private void Refresh()
        {
            Allergens.Clear();
            foreach (var a in allergenService.GetAllAllergens())
                Allergens.Add(a);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
