using Microsoft.Data.SqlClient;
using Restaurant.Model;
using System.Data;
using System.Collections.ObjectModel;

namespace Restaurant.Database
{
    public class BundleService
    {
        private readonly string connectionString = DatabaseHelper.ConnectionString;
        public void AddBundle(Bundle bundle)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("AddBundle", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", bundle.Name);
                cmd.Parameters.AddWithValue("@Price", bundle.Price);
                cmd.Parameters.AddWithValue("@ImagePath", bundle.ImagePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryID", bundle.Category.CategoryID);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBundle(int bundleId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("DeleteBundle", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BundleID", bundleId);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBundle(int bundleId, Bundle bundle)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand deleteCmd = new SqlCommand("DeleteBundleProducts", connection))
                {
                    deleteCmd.CommandType = CommandType.StoredProcedure;
                    deleteCmd.Parameters.AddWithValue("@BundleID", bundleId);
                    deleteCmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand("UpdateBundle", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BundleID", bundleId);
                    cmd.Parameters.AddWithValue("@Name", bundle.Name);
                    cmd.Parameters.AddWithValue("@Price", bundle.Price);
                    cmd.Parameters.AddWithValue("@ImagePath", bundle.ImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", bundle.Category.CategoryID);

                    cmd.ExecuteNonQuery();
                }

                foreach (var bp in bundle.BundleProducts)
                {
                    using (SqlCommand insertCmd = new SqlCommand("AddBundleProduct", connection))
                    {
                        insertCmd.CommandType = CommandType.StoredProcedure;
                        insertCmd.Parameters.AddWithValue("@BundleID", bundleId);
                        insertCmd.Parameters.AddWithValue("@ProductID", bp.Product.ProductID);
                        insertCmd.Parameters.AddWithValue("@Quantity", bp.Quantity);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<Bundle> GetAllBundles()
        {
            var bundles = new List<Bundle>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("GetAllBundles", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                CategoryService categoryService = new CategoryService();

                while (reader.Read())
                {
                    var bundleId = (int)reader["BundleID"];
                    var categoryId = (int)reader["CategoryID"];
                    Category category = categoryService.GetCategoryById(categoryId);

                    var bundle = new Bundle
                    {
                        BundleID = bundleId,
                        Name = reader["Name"].ToString(),
                        ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString(),
                        Category = category,
                        BundleProducts = new ObservableCollection<BundleProduct>(GetBundleProducts(bundleId))
                    };

                    bundles.Add(bundle);
                }
            }

            return bundles;
        }


        public List<BundleProduct> GetBundleProducts(int bundleId)
        {
            var bundleProducts = new List<BundleProduct>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM BundleProducts WHERE BundleID = @BundleID", connection);
                cmd.Parameters.AddWithValue("@BundleID", bundleId);

                SqlDataReader reader = cmd.ExecuteReader();

                ProductService productService = new ProductService();

                while (reader.Read())
                {
                    int productId = (int)reader["ProductID"];
                    Product product = productService.GetProductById(productId);

                    var bp = new BundleProduct
                    {
                        BundleProductID = (int)reader["BundleProductID"],
                        BundleID = bundleId,
                        ProductID = productId,
                        Product = product,
                        Quantity = (int)reader["Quantity"]
                    };

                    bundleProducts.Add(bp);
                }
            }

            return bundleProducts;
        }

        public void AddBundleProducts(int bundleId, List<BundleProduct> products)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var bp in products)
                {
                    using (SqlCommand cmd = new SqlCommand("AddBundleProduct", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BundleID", bundleId);
                        cmd.Parameters.AddWithValue("@ProductID", bp.Product.ProductID);
                        cmd.Parameters.AddWithValue("@Quantity", bp.Quantity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteBundleProducts(int bundleId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("DeleteBundleProducts", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BundleID", bundleId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddBundleProduct(BundleProduct bp)
        {
            AddBundleProduct(bp.BundleID, bp.Product.ProductID, bp.Quantity);
        }

        public void AddBundleProduct(int bundleId, int productId, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("AddBundleProduct", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BundleID", bundleId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public int AddBundleAndReturnId(Bundle bundle)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("AddBundleAndReturnId", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", bundle.Name);
                    cmd.Parameters.AddWithValue("@Price", bundle.Price);
                    cmd.Parameters.AddWithValue("@ImagePath", (object?)bundle.ImagePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", bundle.Category.CategoryID);

                    SqlParameter outputIdParam = new SqlParameter("@NewBundleID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);

                    cmd.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
            }
        }

        public Bundle GetBundleById(int bundleId)
        {
            Bundle bundle = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("GetBundleByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BundleID", bundleId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var categoryService = new CategoryService();
                    var categoryId = (int)reader["CategoryID"];
                    var category = categoryService.GetCategoryById(categoryId);

                    bundle = new Bundle
                    {
                        BundleID = bundleId,
                        Name = reader["Name"].ToString(),
                        Category = category,
                        ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString(),
                        BundleProducts = new ObservableCollection<BundleProduct>(GetBundleProducts(bundleId))
                    };
                }
            }

            return bundle;
        }

    }
}