using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Restaurant.Model;

namespace Restaurant
{
    public class OrderItemToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderItem item)
            {
                string name = item.Product?.Name ?? item.Bundle?.Name ?? "Unknown";
                return $"{name} - {item.Quantity}x - {item.TotalPrice:0.00} lei";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }


    public class OrderItemsToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<OrderItem> items && items.Any())
            {
                var parts = items.Select(item =>
                {
                    string name = item.Product?.Name ?? item.Bundle?.Name ?? "Unknown";
                    return $"{name} - {item.Quantity}x- {item.TotalPrice:0.00} lei";
                });

                return string.Join(Environment.NewLine, parts);
            }

            return "(No items)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
