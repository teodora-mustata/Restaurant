namespace Restaurant.Model
{
    public class Ingredient
    {
        public int IngredientID { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Ingredient other)
                return this.IngredientID == other.IngredientID;
            return false;
        }

        public override int GetHashCode()
        {
            return IngredientID.GetHashCode();
        }
    }
}
