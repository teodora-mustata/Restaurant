using System.Collections.ObjectModel;
using System.Globalization;

namespace Restaurant.Model
{
    public class Bundle
    {
        public int BundleID { get; set; }
        public string Name { get; set; }

        public decimal Price => BundleProducts.Sum(bp => bp.Price) * (decimal)(1-Discount);
        public Category Category { get; set; }

        public string ImagePath { get; set; }

        public ObservableCollection<BundleProduct> BundleProducts { get; set; } = new ObservableCollection<BundleProduct>();

        double Discount = double.Parse(AppConfig.BundleDiscount, CultureInfo.InvariantCulture);

        public bool IsOutOfStock =>
            BundleProducts.Any(bp => bp.Product == null || !bp.Product.HasEnoughStock(bp.Quantity));


    }
}
