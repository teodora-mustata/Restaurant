using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Restaurant.Model;
using Microsoft.Data.SqlClient;

namespace Restaurant.Database
{
    public class OrderService
    {
        private readonly string connectionString = DatabaseHelper.ConnectionString;

        public int AddOrder(Order order)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("AddOrder", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClientID", order.ClientID);
                cmd.Parameters.AddWithValue("@DatePlaced", order.DatePlaced);
                cmd.Parameters.AddWithValue("@EstimatedDeliveryTime", order.EstimatedDeliveryTime);
                cmd.Parameters.AddWithValue("@Status", (int)order.Status);
                cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

                int newOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                return newOrderId;
            }
        }

        public void AddOrderItem(int orderId, OrderItem item)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("AddOrderItem", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@ProductID", item.Product?.ProductID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BundleID", item.Bundle?.BundleID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateOrder(Order order)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UpdateOrder", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                cmd.Parameters.AddWithValue("@Status", (int)order.Status);
                cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                cmd.Parameters.AddWithValue("@EstimatedDeliveryTime", order.EstimatedDeliveryTime);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteOrder(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DeleteOrder", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetAllOrders", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var order = new Order
                    {
                        OrderID = (int)reader["OrderID"],
                        ClientID = (int)reader["ClientID"],
                        DatePlaced = (DateTime)reader["DatePlaced"],
                        EstimatedDeliveryTime = (DateTime)reader["EstimatedDeliveryTime"],
                        Status = (OrderStatus)(int)reader["Status"],
                        TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                    };
                    orders.Add(order);
                }
            }

            foreach (var order in orders)
            {
                order.Items = new ObservableCollection<OrderItem>(GetOrderItemsByOrderId(order.OrderID));
            }

            return orders;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            var items = new List<OrderItem>();
            var productService = new ProductService();
            var bundleService = new BundleService();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetOrderItemsByOrderId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", orderId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product? product = reader["ProductID"] != DBNull.Value ? productService.GetProductById((int)reader["ProductID"]) : null;
                    Bundle? bundle = reader["BundleID"] != DBNull.Value ? bundleService.GetBundleById((int)reader["BundleID"]) : null;

                    var item = new OrderItem
                    {
                        OrderItemID = (int)reader["OrderItemID"],
                        Product = product,
                        Bundle = bundle,
                        Quantity = (int)reader["Quantity"]
                    };
                    items.Add(item);
                }
            }

            return items;
        }

        public int GetClientIdByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("GetClientIdByEmail", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public List<Order> GetOrdersByClientId(int clientId)
        {
            var orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("GetOrdersByClientId", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            OrderID = (int)reader["OrderID"],
                            ClientID = (int)reader["ClientID"],
                            DatePlaced = (DateTime)reader["DatePlaced"],
                            EstimatedDeliveryTime = (DateTime)reader["EstimatedDeliveryTime"],
                            Status = (OrderStatus)(int)reader["Status"],
                            TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                        });
                    }
                }
            }

            foreach (var order in orders)
            {
                order.Items = new ObservableCollection<OrderItem>(GetOrderItemsByOrderId(order.OrderID));
            }

            return orders;
        }


        public void PlaceOrder(Order order)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    int clientId = GetClientIdByEmail(Session.Email);
                    order.ClientID = clientId;

                    List<Order> previousOrders = GetOrdersByClientId(clientId);

                    var discountManager = new OrderDiscountManager();
                    discountManager.ProcessOrder(order, previousOrders);

                    SqlCommand cmd = new SqlCommand("AddOrder", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClientID", order.ClientID);
                    cmd.Parameters.AddWithValue("@DatePlaced", order.DatePlaced);
                    cmd.Parameters.AddWithValue("@EstimatedDeliveryTime", order.EstimatedDeliveryTime);
                    cmd.Parameters.AddWithValue("@Status", (int)order.Status);
                    cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

                    int orderId = Convert.ToInt32(cmd.ExecuteScalar());

                    foreach (var item in order.Items)
                    {
                        SqlCommand itemCmd = new SqlCommand("AddOrderItem", connection, transaction);
                        itemCmd.CommandType = CommandType.StoredProcedure;
                        itemCmd.Parameters.AddWithValue("@OrderID", orderId);
                        itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                        if (item.Product != null)
                        {
                            itemCmd.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                            itemCmd.Parameters.AddWithValue("@BundleID", DBNull.Value);
                        }
                        else
                        {
                            itemCmd.Parameters.AddWithValue("@ProductID", DBNull.Value);
                            itemCmd.Parameters.AddWithValue("@BundleID", item.Bundle.BundleID);
                        }

                        itemCmd.ExecuteNonQuery();

                        if (item.Product != null)
                        {
                            SqlCommand stockCmd = new SqlCommand("UpdateProductStock", connection, transaction);
                            stockCmd.CommandType = CommandType.StoredProcedure;
                            stockCmd.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                            stockCmd.Parameters.AddWithValue("@Quantity", item.Quantity * item.Product.QuantityPerServing);
                            stockCmd.ExecuteNonQuery();
                        }
                        else if (item.Bundle != null)
                        {
                            foreach (var bp in item.Bundle.BundleProducts)
                            {
                                if (bp.Product == null) continue;

                                SqlCommand stockCmd = new SqlCommand("UpdateProductStock", connection, transaction);
                                stockCmd.CommandType = CommandType.StoredProcedure;
                                stockCmd.Parameters.AddWithValue("@ProductID", bp.Product.ProductID);
                                stockCmd.Parameters.AddWithValue("@Quantity", item.Quantity * bp.Quantity);
                                stockCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CancelOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("CancelOrder", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
