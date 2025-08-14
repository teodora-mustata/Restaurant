using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Model;

public class UserService
{
    private readonly string connectionString = DatabaseHelper.ConnectionString;

    public void AddUser(string firstName, string lastName, string email, string password, string phoneNumber, string address, string role)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("AddUser", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", firstName);
            cmd.Parameters.AddWithValue("@LastName", lastName);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.Parameters.AddWithValue("@Address", address);
            cmd.Parameters.AddWithValue("@Role", role);

            cmd.ExecuteNonQuery();
        }
    }

    public (string role, string firstName) LoginUser(string email, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("LoginUser", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (reader["Role"].ToString(), reader["FirstName"].ToString());
            }
            else
            {
                return (null, null);
            }
        }
    }

    public User GetUserById(int id)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmd = new SqlCommand("GetUserById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", id);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User
                    {
                        Id = id,
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        Role = reader["Role"].ToString(),
                        Password = null
                    };
                }
            }
        }

        return null;
    }

}
