﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ProductController(IConfiguration config)
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

        //Get All
        [HttpGet]

        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description
                        FROM Product";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new List<Product>();

                    while (reader.Read())
                    {
                       Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                        };

                        products.Add(product);

                    }
                    reader.Close();

                    return Ok(products);
                }

            }
        }


        //GET BY ID
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
                        FROM Product p
                        WHERE p.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Product product = null;

                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                           
                        };
                    }
                    reader.Close();

                    return Ok(product);
                }
            }
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Product (DateAdded, ProductTypeId, CustomerId, Price, Title, Description)
                                        OUTPUT INSERTED.Id
                                        VALUES (@dateAdded, @productTypeId, @customerId, @price, @title, @description)";
                    cmd.Parameters.Add(new SqlParameter("@dateAdded", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));

                    int newId = (int)cmd.ExecuteScalar();
                    product.Id = newId;
                    product.DateAdded = DateTime.Now;
                    return CreatedAtRoute("GetProduct", new { id = newId }, product);
                }
            }
        }

        // Put
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Product
                                            SET ProductTypeId = @productTypeId,
                                            CustomerId = @customerId,
                                            Price = @price,
                                            Title = @title,
                                            Description = @description
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@description", product.Description));
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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //Check method
        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description
                        FROM Product
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}
