using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Model;

namespace Restaurant.Database
{
    public class AllergenService
    {
        private readonly string connectionString = DatabaseHelper.ConnectionString;

        public void AddAllergen(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("AddAllergen", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Allergen> GetAllAllergens()
        {
            var allergens = new List<Allergen>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Allergens", connection);
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
        public void UpdateAllergen(int id, string newName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("UpdateAllergen", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AllergenID", id);
                cmd.Parameters.AddWithValue("@Name", newName);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAllergen(int allergenId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("DeleteAllergen", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AllergenID", allergenId);

                cmd.ExecuteNonQuery();
            }
        }


    }
}
