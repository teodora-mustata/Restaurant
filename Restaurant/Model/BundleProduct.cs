using System.Windows.Controls;

namespace Restaurant.Model
{
    public class BundleProduct
    {
        public int BundleProductID { get; set; }
        public int BundleID { get; set; }
        public int ProductID { get; set; }
        public Bundle Bundle { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal Price => Product.PricePerGram * (decimal)Quantity;
    }
}
