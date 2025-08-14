using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Restaurant.Model;
using Restaurant.Database;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Restaurant.ViewModel
{
    public class ShowClientOrderHistoryViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService = new();

        public ObservableCollection<Order> AllOrders { get; set; } = new();
        public ObservableCollection<Order> ActiveOrders { get; set; } = new();

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
                OnPropertyChanged(nameof(IsSelectedOrderCancelable));
            }
        }

        public bool IsSelectedOrderCancelable =>
            SelectedOrder != null &&
            (SelectedOrder.Status == OrderStatus.Registered ||
             SelectedOrder.Status == OrderStatus.Preparing ||
             SelectedOrder.Status == OrderStatus.Delivering);

        public ICommand CancelOrderCommand => new RelayCommand(CancelSelectedOrder, () => IsSelectedOrderCancelable);

        private void CancelSelectedOrder()
        {
            if (SelectedOrder == null) return;

            var result = MessageBox.Show("Are you sure you want to cancel this order?", "Cancel Order", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _orderService.CancelOrder(SelectedOrder.OrderID);
                SelectedOrder.Status = OrderStatus.Cancelled;

                // Mută comanda din lista activă în AllOrders
                ActiveOrders.Remove(SelectedOrder);
                if (!AllOrders.Contains(SelectedOrder))
                    AllOrders.Add(SelectedOrder);

                MessageBox.Show("Order cancelled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                OnPropertyChanged(nameof(IsSelectedOrderCancelable));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cancelling order: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ShowClientOrderHistoryViewModel()
        {
            try
            {
                int clientId = _orderService.GetClientIdByEmail(Session.Email);
                var orders = _orderService.GetOrdersByClientId(clientId);

                foreach (var order in orders.OrderByDescending(o => o.DatePlaced))
                {
                    order.Items = new ObservableCollection<OrderItem>(_orderService.GetOrderItemsByOrderId(order.OrderID));
                    AllOrders.Add(order);

                    if (order.Status != OrderStatus.Cancelled && order.Status != OrderStatus.DeliverySuccessful)
                        ActiveOrders.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading orders: " + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
