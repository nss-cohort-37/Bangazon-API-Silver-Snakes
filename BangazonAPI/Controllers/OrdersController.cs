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
    public class OrdersController : Controller
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
                return new SqlConnection(_config.GetConnectionString("BangazonAPI"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersByCustomerId([FromQuery] Int32 customerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId 
                                        FROM Order 
                                        WHERE CustomerId = @customerId";
                    cmd.Parameters.Add(new SqlParameter("@customerId", customerId));

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Order> orders = new List<Order>();

                    while (reader.Read())
                    {
                        Order order = new Order()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                        {
                            order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                        }
                        orders.Add(order);
                    }
                    reader.Close();

                    return Ok(orders);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId 
                                        FROM Order 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Order order = null;

                    if (reader.Read())
                    {
                        order = new Order()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                        {
                            order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                        }
                    }
                    reader.Close();

                    return Ok(order);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomersShoppingCart(
            [FromQuery] Int32 customerId,
            [FromQuery] bool cart)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId 
                                        FROM Order 
                                        WHERE CustomerId = @customerId";
                    cmd.Parameters.Add(new SqlParameter("@customerId", customerId));

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Order> orders = new List<Order>();

                    Order order = null;

                    while (reader.Read())
                    {
                        if (order == null)
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
                        }

                        if (cart == true)
                        {
                            order.Products.Add(new Product()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                            });
                        }
                    }
                    reader.Close();

                    return Ok(orders);
                }
            }
        }
    }
}
