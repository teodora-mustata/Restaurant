using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Restaurant.Model;
using Restaurant.Database;

namespace Restaurant.ViewModel
{
    public class ShowEmployeeOrderHistoryViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService = new();
        private readonly UserService _userService = new(); 

        public ObservableCollection<Order> AllOrders { get; set; } = new();
        public ObservableCollection<Order> ActiveOrders { get; set; } = new();

        public IEnumerable<OrderStatus> StatusValues => Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));

                if (_selectedOrder != null)
                    SelectedStatus = _selectedOrder.Status;

                (UpdateStatusCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsSelectedOrderCancelable =>
            SelectedOrder != null &&
            (SelectedOrder.Status == OrderStatus.Registered ||
             SelectedOrder.Status == OrderStatus.Preparing ||
             SelectedOrder.Status == OrderStatus.Delivering);

        private OrderStatus _selectedStatus;
        public OrderStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
                (UpdateStatusCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand UpdateStatusCommand => new RelayCommand(UpdateOrderStatus, () => SelectedOrder != null);

        private void UpdateOrderStatus()
        {
            if (SelectedOrder == null) return;

            try
            {
                SelectedOrder.Status = SelectedStatus;
                _orderService.UpdateOrder(SelectedOrder);

                if (SelectedStatus == OrderStatus.DeliverySuccessful || SelectedStatus == OrderStatus.Cancelled)
                {
                    if (ActiveOrders.Contains(SelectedOrder))
                        ActiveOrders.Remove(SelectedOrder);
                }

                MessageBox.Show("Order status updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                OnPropertyChanged(nameof(AllOrders));
                OnPropertyChanged(nameof(ActiveOrders));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating order: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public ShowEmployeeOrderHistoryViewModel()
        {
            try
            {
                var allOrders = _orderService.GetAllOrders();

                foreach (var order in allOrders.OrderByDescending(o => o.DatePlaced))
                {
                    var user = _userService.GetUserById(order.ClientID);

                    AllOrders.Add(order);

                    if (order.Status != OrderStatus.Cancelled &&
                        order.Status != OrderStatus.DeliverySuccessful)
                    {
                        ActiveOrders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading orders: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
