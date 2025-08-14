using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Model
{
    public enum OrderStatus
    {
        Registered,
        Preparing,
        Delivering,
        DeliverySuccessful,
        Cancelled
    }

    public class Order : INotifyPropertyChanged
    {
        public int OrderID { get; set; }
        public int ClientID { get; set; }
        public DateTime DatePlaced { get; set; }

        public DateTime EstimatedDeliveryTime { get; set; }
        //public OrderStatus Status { get; set; }

        private OrderStatus _status;
        public OrderStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public decimal TotalPrice { get; set; }

        public ObservableCollection<OrderItem> Items { get; set; } = new ObservableCollection<OrderItem>();
        public Order()
        {
            DatePlaced = DateTime.Now;
            EstimatedDeliveryTime = DatePlaced.AddHours(2);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
