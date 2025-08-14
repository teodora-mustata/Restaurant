using System.ComponentModel;
using Restaurant.Model;

namespace Restaurant.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _loggedUserDisplay;

        public MainWindowViewModel()
        {
            Session.LoggedUserChanged += OnSessionChanged;
            UpdateLoggedUserDisplay();
        }

        public string LoggedUserDisplay
        {
            get => _loggedUserDisplay;
            set
            {
                if (_loggedUserDisplay != value)
                {
                    _loggedUserDisplay = value;
                    OnPropertyChanged(nameof(LoggedUserDisplay)); 
                }
            }
        }

        public void UpdateLoggedUserDisplay()
        {
            if (!string.IsNullOrEmpty(Session.FirstName) && !string.IsNullOrEmpty(Session.Role) && !string.IsNullOrEmpty(Session.Email))
            {
                LoggedUserDisplay = $"Logged in: {Session.Role} {Session.FirstName}";
            }
            else
            {
                LoggedUserDisplay = string.Empty;
            }
        }


        private void OnSessionChanged(string propertyName)
        {
            if (propertyName == nameof(Session.FirstName) || propertyName == nameof(Session.Role))
            {
                UpdateLoggedUserDisplay();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
