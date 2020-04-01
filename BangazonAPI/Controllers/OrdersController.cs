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
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

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
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                });
                            }
                            //else
                            //{
                            //    return new StatusCodeResult(StatusCodes.Status204NoContent);
                            //}
                        }
                        reader.Close();
                        return Ok(order);
                    } 
                    else 
                    { 
                        cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId 
                                            FROM [Order] 
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
        }
        
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, UserPaymentTypeId 
                                        FROM [Order] 
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
    }
}
