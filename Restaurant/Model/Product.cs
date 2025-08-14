using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Restaurant.Model
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int QuantityPerServing { get; set; }
        public int TotalQuantity { get; set; }
        public bool IsVegan { get; set; }
        public bool IsVegetarian { get; set; }
        public Category Category { get; set; }
        public string ImagePath { get; set; }

        public ObservableCollection<Allergen> Allergens { get; set; }
        public ObservableCollection<Ingredient> Ingredients { get; set; }

        public Product()
        {
            Allergens = new ObservableCollection<Allergen>();
            Ingredients = new ObservableCollection<Ingredient>();
        }

        public decimal PricePerGram => QuantityPerServing> 0 ? Price / (decimal)QuantityPerServing : 0;

        public bool IsOutOfStock => !HasEnoughStock(QuantityPerServing);

        public bool HasEnoughStock(int requiredQuantity)
        {
            return TotalQuantity >= requiredQuantity;
        }


    }

}
