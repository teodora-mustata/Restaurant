using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Model
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public Product? Product { get; set; }
        public Bundle? Bundle { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice
        {
            get
            {
                if (Product != null)
                    return Product.Price * Quantity;
                if (Bundle != null)
                    return Bundle.Price * Quantity;
                return 0;
            }
        }



    }

}
