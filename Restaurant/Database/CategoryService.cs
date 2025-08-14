using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Model;

namespace Restaurant.Database
{
    public class CategoryService
    {
        private readonly string connectionString = DatabaseHelper.ConnectionString;

        public void AddCategory(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("AddCategory", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Categories", connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryID = (int)reader["CategoryID"],
                        Name = reader["Name"].ToString()
                    });
                }
            }

            return categories;
        }
        public void UpdateCategory(int id, string newName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("UpdateCategory", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", id);
                cmd.Parameters.AddWithValue("@Name", newName);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("DeleteCategory", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);

                cmd.ExecuteNonQuery();
            }
        }

        public Category GetCategoryById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Categories WHERE CategoryID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Category
                        {
                            CategoryID = (int)reader["CategoryID"],
                            Name = reader["Name"].ToString()
                        };
                    }
                }
            }

            return null; 
        }


    }
}
