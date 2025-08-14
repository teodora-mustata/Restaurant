namespace Restaurant.Model
{
    public class Allergen
    {
        public int AllergenID { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Allergen other)
                return this.AllergenID == other.AllergenID;
            return false;
        }

        public override int GetHashCode()
        {
            return AllergenID.GetHashCode();
        }
    }
}
