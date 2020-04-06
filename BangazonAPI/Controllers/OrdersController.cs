using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        /// <summary>
        /// Get orders by a customer's Id. Can query to see a customers shopping cart.
        /// </summary>
        /// <param name="customerId">Any valid customer Id.</param>
        /// <param name="cart">If true, will return the customer's shopping cart if they have one active.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrdersByCustomerId(
            [FromQuery] Int32? customerId,
            [FromQuery] bool cart)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (cart == true)
                    {
                        cmd.CommandText = @"SELECT o.Id, o.CustomerId, o.UserPaymentTypeId, 
                                                op.Id, op.OrderId, op.ProductId, 
                                                p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
                                                FROM [Order] o 
                                                LEFT JOIN OrderProduct op ON o.Id = op.OrderId
                                                LEFT JOIN Product p ON op.ProductId = p.Id
                                                WHERE o.CustomerId = @customerId";
                        cmd.Parameters.Add(new SqlParameter("@customerId", customerId));

                        SqlDataReader reader = cmd.ExecuteReader();

                        Order order = null;

                        while (reader.Read())
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                            {

                                if (order == null)
                                {
                                    order = new Order()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        Products = new List<Product>()
                                    };
                                }

                                order.Products.Add(new Product()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                });
                            }
                        }
                        reader.Close();

                        return Ok(order);
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT o.Id, o.CustomerId, o.UserPaymentTypeId, 
                                                op.Id, op.OrderId, op.ProductId, 
                                                p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
                                                FROM [Order] o 
                                                LEFT JOIN OrderProduct op ON o.Id = op.OrderId
                                                LEFT JOIN Product p ON op.ProductId = p.Id
                                                WHERE o.CustomerId = @customerId";
                        cmd.Parameters.Add(new SqlParameter("@customerId", customerId));

                        SqlDataReader reader = cmd.ExecuteReader();

                        List<Order> orders = new List<Order>();


                        while (reader.Read())
                        {
                            var existingOrder = orders.FirstOrDefault(order => order.Id == reader.GetInt32(reader.GetOrdinal("Id")));

                            if (existingOrder == null)
                            {
                                Order order = new Order()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    Products = new List<Product>()
                                };
                                if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                                {
                                    order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                                }

                                order.Products.Add(new Product()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                });
                                orders.Add(order);
                            }
                            else
                            {
                                existingOrder.Products.Add(new Product()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                });
                            }
                        }
                        reader.Close();

                        return Ok(orders);
                    }
                }
            }
        }

        /// <summary>
        /// Get an order by an Id.
        /// </summary>
        /// <param name="id">Will return the specified order if Id is valid with an array of products purchased on the order.</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id, o.CustomerId, o.UserPaymentTypeId, 
                                                op.Id, op.OrderId, op.ProductId, 
                                                p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
                                                FROM [Order] o 
                                                LEFT JOIN OrderProduct op ON o.Id = op.OrderId
                                                LEFT JOIN Product p ON op.ProductId = p.Id
                                        WHERE o.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Order order = null;

                    while (reader.Read())
                    {
                        if (order == null)
                        {
                            order = new Order()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Products = new List<Product>()
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                            {
                                order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                            }
                        }

                        order.Products.Add(new Product()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description"))
                        });
                    }
                    reader.Close();

                    return Ok(order);
                }
            }
        }

        /// <summary>
        /// Add a product to an active shopping cart, or create a shopping cart for the product to go in.
        /// </summary>
        /// <param name="custProd">Takes the purchasing customerId as well as the productId from a CustomerProduct object.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerProduct custProd)
        {
            int orderId = 0;

            // 1. Find the customer's shopping cart if there is one.
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId
                                        FROM [Order]
                                        WHERE CustomerId = @customerId AND UserPaymentTypeId IS NULL";
                    cmd.Parameters.Add(new SqlParameter("@customerId", custProd.CustomerId));

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        orderId = reader.GetInt32(reader.GetOrdinal("Id"));
                    }
                    reader.Close();
                }
            }

            // 2. If they don't have a shopping cart, create one.
            if (orderId == 0)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO [Order] (CustomerId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@customerId)";
                        cmd.Parameters.Add(new SqlParameter("@customerId", custProd.CustomerId));

                        orderId = (int)cmd.ExecuteScalar();
                    }
                }
            }

            // 3. Add a record to OrderProduct
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO OrderProduct (OrderId, ProductId)
                                        VALUES (@orderId, @productId)";
                    cmd.Parameters.Add(new SqlParameter("@orderId", orderId));
                    cmd.Parameters.Add(new SqlParameter("@productId", custProd.ProductId));

                    cmd.ExecuteNonQuery();
                    return Ok(new Order()
                    {
                        Id = orderId,
                        CustomerId = custProd.CustomerId
                    });
                }
            }
        }

        /// <summary>
        /// Purchase order in customer's cart.
        /// </summary>
        /// <param name="id">Any valid orderId of an active shopping cart.</param>
        /// <param name="order">Takes the customerId and userPaymentTypeId from an order object from the body.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE [Order]
                                            SET CustomerId = @customerId,
                                            UserPaymentTypeId = @userPaymentTypeId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@userPaymentTypeId", order.UserPaymentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Remove a product from an active shopping cart.
        /// </summary>
        /// <param name="orderId">OrderId of the active shopping cart.</param>
        /// <param name="productId">ProductId of the product you want removed from the shopping cart.</param>
        /// <returns></returns>
        [HttpDelete("{orderId}/products{productId}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int orderId,
            [FromRoute] int productId
            )
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId
                                        FROM [Order]
                                        WHERE Id = @orderId AND UserPaymentTypeId IS NULL";
                        cmd.Parameters.Add(new SqlParameter("@orderId", orderId));

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            using (SqlConnection connection = Connection)
                            {
                                connection.Open();
                                using (SqlCommand command = connection.CreateCommand())
                                {
                                    command.CommandText = @"DELETE FROM OrderProduct WHERE OrderId = @orderId AND ProductId = @productId";
                                    command.Parameters.Add(new SqlParameter("@orderId", orderId));
                                    command.Parameters.Add(new SqlParameter("@productId", productId));

                                    int rowsAffected = command.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                                    }
                                    throw new Exception("No rows affected");
                                }
                            }
                        }
                        return new StatusCodeResult(StatusCodes.Status304NotModified);
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(orderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, CustomerId
                        FROM [Order]
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
