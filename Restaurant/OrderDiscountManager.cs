//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Restaurant.Model;

//namespace Restaurant
//{
//    public class OrderDiscountManager
//    {
//        private readonly decimal _deliveryFee;
//        private readonly decimal _minOrderForFreeDelivery;
//        private readonly decimal _minOrderForValueDiscount;
//        private readonly double _valueDiscountPercentage;
//        private readonly int _minOrdersInTimeframeForDiscount;
//        private readonly TimeSpan _timeframeForOrderCountDiscount;
//        private readonly double _orderCountDiscountPercentage;

//        public OrderDiscountManager()
//        {
//            _deliveryFee = decimal.Parse(AppConfig.DeliveryFee, CultureInfo.InvariantCulture);
//            _minOrderForFreeDelivery = decimal.Parse(AppConfig.MinOrderForFreeDelivery, CultureInfo.InvariantCulture);
//            _minOrderForValueDiscount = decimal.Parse(AppConfig.MinOrderForValueDiscount, CultureInfo.InvariantCulture);
//            _valueDiscountPercentage = double.Parse(AppConfig.ValueDiscountPercentage, CultureInfo.InvariantCulture);
//            _minOrdersInTimeframeForDiscount = int.Parse(AppConfig.MinOrdersInTimeframeForDiscount);
//            _timeframeForOrderCountDiscount = TimeSpan.Parse(AppConfig.TimeframeForOrderCountDiscount, CultureInfo.InvariantCulture);
//            _orderCountDiscountPercentage = double.Parse(AppConfig.OrderCountDiscountPercentage, CultureInfo.InvariantCulture);
//        }

//        public void ProcessOrder(Order order, IEnumerable<Order> previousOrders)
//        {
//            decimal basePrice = order.Items.Sum(item => item.TotalPrice);
//            decimal discountedPrice = basePrice;

//            if (basePrice >= _minOrderForValueDiscount)
//            {
//                discountedPrice -= basePrice * (decimal)_valueDiscountPercentage;
//            }

//            var now = DateTime.Now;
//            var recentOrders = previousOrders.Where(o =>
//                o.ClientID == order.ClientID &&
//                o.DatePlaced >= now - _timeframeForOrderCountDiscount
//            );

//            if (recentOrders.Count() >= _minOrdersInTimeframeForDiscount)
//            {
//                discountedPrice -= discountedPrice * (decimal)_orderCountDiscountPercentage;
//            }

//            decimal deliveryFee = discountedPrice >= _minOrderForFreeDelivery ? 0 : _deliveryFee;

//            order.TotalPrice = discountedPrice + deliveryFee;
//        }
//    }
//}

using Restaurant.Model;
using Restaurant;
using System.Globalization;

public class OrderDiscountManager
{
    private readonly decimal _deliveryFee;
    private readonly decimal _minOrderForFreeDelivery;
    private readonly decimal _minOrderForValueDiscount;
    private readonly decimal _valueDiscountPercentage;   
    private readonly int _minOrdersInTimeframeForDiscount;
    private readonly TimeSpan _timeframeForOrderCountDiscount;
    private readonly decimal _orderCountDiscountPercentage;  

    public OrderDiscountManager()
    {
        _deliveryFee = decimal.Parse(AppConfig.DeliveryFee, CultureInfo.InvariantCulture);
        _minOrderForFreeDelivery = decimal.Parse(AppConfig.MinOrderForFreeDelivery, CultureInfo.InvariantCulture);
        _minOrderForValueDiscount = decimal.Parse(AppConfig.MinOrderForValueDiscount, CultureInfo.InvariantCulture);
        _valueDiscountPercentage = decimal.Parse(AppConfig.ValueDiscountPercentage, CultureInfo.InvariantCulture);
        _minOrdersInTimeframeForDiscount = int.Parse(AppConfig.MinOrdersInTimeframeForDiscount);
        _timeframeForOrderCountDiscount = TimeSpan.Parse(AppConfig.TimeframeForOrderCountDiscount, CultureInfo.InvariantCulture);
        _orderCountDiscountPercentage = decimal.Parse(AppConfig.OrderCountDiscountPercentage, CultureInfo.InvariantCulture);
    }

    public void ProcessOrder(Order order, IEnumerable<Order> previousOrders)
    {
        decimal basePrice = order.Items.Sum(item => item.TotalPrice);
        decimal discountedPrice = basePrice;

        if (basePrice >= _minOrderForValueDiscount)
        {
            discountedPrice -= basePrice * _valueDiscountPercentage; 
        }

        var now = DateTime.Now;
        var recentOrders = previousOrders.Where(o =>
            o.ClientID == order.ClientID &&
            o.DatePlaced >= now - _timeframeForOrderCountDiscount
        );

        if (recentOrders.Count() >= _minOrdersInTimeframeForDiscount)
        {
            discountedPrice -= discountedPrice * _orderCountDiscountPercentage;
        }

        decimal deliveryFee = discountedPrice >= _minOrderForFreeDelivery ? 0 : _deliveryFee;
        order.TotalPrice = discountedPrice + deliveryFee;
    }
}
