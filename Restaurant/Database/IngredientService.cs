using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Model;

namespace Restaurant.Database
{
    public class IngredientService
    {
        private readonly string connectionString = DatabaseHelper.ConnectionString;

        public void AddIngredient(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("AddIngredient", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Ingredient> GetAllIngredients()
        {
            var ingredients = new List<Ingredient>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Ingredients", connection);
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

        public void UpdateIngredient(int id, string newName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("UpdateIngredient", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IngredientID", id);
                cmd.Parameters.AddWithValue("@Name", newName);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteIngredient(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("DeleteIngredient", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IngredientID", id);

                cmd.ExecuteNonQuery();
            }
        }


    }
}
