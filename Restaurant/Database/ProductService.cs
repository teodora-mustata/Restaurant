using Microsoft.Data.SqlClient;
using Restaurant.Database;
using Restaurant.Model;
using System.Collections.ObjectModel;
using System.Data;

public class ProductService
{
    private readonly string connectionString = DatabaseHelper.ConnectionString;

    public int AddProduct(Product product)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("AddProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@QuantityPerServing", product.QuantityPerServing);
            cmd.Parameters.AddWithValue("@TotalQuantity", product.TotalQuantity);
            cmd.Parameters.AddWithValue("@IsVegan", product.IsVegan);
            cmd.Parameters.AddWithValue("@IsVegetarian", product.IsVegetarian);
            cmd.Parameters.AddWithValue("@CategoryID", product.Category.CategoryID);
            cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath ?? (object)DBNull.Value);

            conn.Open();
            int newProductId = Convert.ToInt32(cmd.ExecuteScalar());
            return newProductId;
        }
    }


    public void UpdateProduct(Product product)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("UpdateProduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@QuantityPerServing", product.QuantityPerServing);
            cmd.Parameters.AddWithValue("@TotalQuantity", product.TotalQuantity);
            cmd.Parameters.AddWithValue("@IsVegan", product.IsVegan);
            cmd.Parameters.AddWithValue("@IsVegetarian", product.IsVegetarian);
            cmd.Parameters.AddWithValue("@CategoryID", product.Category.CategoryID);
            cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }
    }

    public void DeleteProduct(int productId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("DeleteProduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            cmd.ExecuteNonQuery();
        }
    }
    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("GetAllProducts", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = cmd.ExecuteReader();

            CategoryService categoryService = new CategoryService();

            while (reader.Read())
            {
                var productId = (int)reader["ProductID"];
                var categoryId = (int)reader["CategoryID"];
                Category category = categoryService.GetCategoryById(categoryId);

                var product = new Product
                {
                    ProductID = productId,
                    Name = reader["Name"].ToString(),
                    Price = (decimal)reader["Price"],
                    QuantityPerServing = (int)reader["QuantityPerServing"],
                    TotalQuantity = (int)reader["TotalQuantity"],
                    IsVegan = (bool)reader["IsVegan"],
                    IsVegetarian = (bool)reader["IsVegetarian"],
                    Category = category,
                    ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString(),
                    Ingredients = new ObservableCollection<Ingredient>(GetProductIngredients(productId)),
                    Allergens = new ObservableCollection<Allergen>(GetProductAllergens(productId))
                };

                products.Add(product);
            }
        }

        return products;
    }




    public void AddIngredientToProduct(int productId, int ingredientId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("AddProductIngredient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);
            cmd.Parameters.AddWithValue("@IngredientID", ingredientId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void AddAllergenToProduct(int productId, int allergenId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("AddProductAllergen", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);
            cmd.Parameters.AddWithValue("@AllergenID", allergenId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public List<Ingredient> GetProductIngredients(int productId)
    {
        var ingredients = new List<Ingredient>();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("GetProductIngredients", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ingredients.Add(new Ingredient
                {
                    IngredientID = (int)reader["IngredientID"],
                    Name = reader["Name"].ToString()
                });
            }
        }

        return ingredients;
    }

    public List<Allergen> GetProductAllergens(int productId)
    {
        var allergens = new List<Allergen>();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("GetProductAllergens", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                allergens.Add(new Allergen
                {
                    AllergenID = (int)reader["AllergenID"],
                    Name = reader["Name"].ToString()
                });
            }
        }

        return allergens;
    }

    public void UpdateProductIngredients(int productId, ObservableCollection<Ingredient> ingredients)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand clearCmd = new SqlCommand("UpdateProductIngredients", conn);
            clearCmd.CommandType = CommandType.StoredProcedure;
            clearCmd.Parameters.AddWithValue("@ProductID", productId);
            clearCmd.ExecuteNonQuery();

            foreach (var ingredient in ingredients)
            {
                AddIngredientToProduct(productId, ingredient.IngredientID);
            }
        }
    }

    public void UpdateProductAllergens(int productId, ObservableCollection<Allergen> allergens)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand clearCmd = new SqlCommand("UpdateProductAllergens", conn);
            clearCmd.CommandType = CommandType.StoredProcedure;
            clearCmd.Parameters.AddWithValue("@ProductID", productId);
            clearCmd.ExecuteNonQuery();

            foreach (var allergen in allergens)
            {
                AddAllergenToProduct(productId, allergen.AllergenID);
            }
        }
    }

    public Product GetProductById(int productId)
    {
        Product product = null;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("GetProductByID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var categoryService = new CategoryService();
                var categoryId = (int)reader["CategoryID"];
                var category = categoryService.GetCategoryById(categoryId);

                product = new Product
                {
                    ProductID = productId,
                    Name = reader["Name"].ToString(),
                    Price = (decimal)reader["Price"],
                    QuantityPerServing = (int)reader["QuantityPerServing"],
                    TotalQuantity = (int)reader["TotalQuantity"],
                    IsVegan = (bool)reader["IsVegan"],
                    IsVegetarian = (bool)reader["IsVegetarian"],
                    ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString(),
                    Category = category,
                    Ingredients = new ObservableCollection<Ingredient>(GetProductIngredients(productId)),
                    Allergens = new ObservableCollection<Allergen>(GetProductAllergens(productId))
                };
            }
        }

        return product;
    }



}
