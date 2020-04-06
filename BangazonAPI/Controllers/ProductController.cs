using System;
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
        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="q">Query String to search for a word in the Title or Description, will return anything containing the search term</param>
        /// <param name="OrderBy">To order by Popularity put "popularity" to order by Recent put "recent" to order by Price put "price"</param>
        /// <param name="asc">When ordering by "price" if True will order by ascending, if false will order by descending</param>
        /// <returns>List of available products</returns>
        //Get All
        [HttpGet]

        public async Task<IActionResult> Get([FromQuery] string q, [FromQuery] string OrderBy, [FromQuery] bool asc)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description
                        FROM Product
                        WHERE 1 = 1";

                    if (q != null)
                    {
                        cmd.CommandText += " AND Title LIKE @title OR Description LIKE @description";
                        cmd.Parameters.Add(new SqlParameter("@Title", "%" + q + "%"));
                        cmd.Parameters.Add(new SqlParameter("@Description", "%" + q + "%"));
                    }
                    if (OrderBy == "recent")
                    {
                        cmd.CommandText = @"SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description
                        FROM Product
                        WHERE 1 = 1
                        ORDER BY DateAdded DESC";
                    }
                    if (OrderBy == "popularity")
                    {
                        cmd.CommandText = @"SELECT p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.[Description], Count(p.Id) AS Count
                            FROM Product p
                            LEFT JOIN OrderProduct op
                            ON op.ProductId = p.Id
                            GROUP By p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.[Description]
                            ORDER By Count Desc";
                    }
                    if (OrderBy == "price" &&  asc == true)
                    {
                        cmd.CommandText = @"SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description FROM Product
                                            ORDER BY Price ASC";
                    }
                    if (OrderBy == "price" && asc == false)
                    {
                        cmd.CommandText = @"SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description FROM Product
                                            ORDER BY Price DESC";
                    }
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

        /// <summary>
        /// Get Products by ID
        /// </summary>
        /// <param name="id">Search all products by Id by entering product Id integer</param>
        /// <returns>The Individual product associated with that Id</returns>
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
        /// <summary>
        /// Posts a new product
        /// </summary>
        /// <param name="product">Enter the fields to create a new product \br ProductTypeId \br Customer Id \br Price \br Title 
        ///  Description \br</param>
        /// <returns>The new instance of the created product</returns>
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
        /// <summary>
        /// Can edit each prodcut
        /// </summary>
        /// <param name="id">Finds the product by Id to be edited, Id must be included to edit</param>
        /// <param name="product">allows you to edit the product information</param>
        /// <returns>the New product and updated properties</returns>
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
        /// <summary>
        /// Product can be deleted
        /// </summary>
        /// <param name="id">Finds the product by the Id for deletion</param>
        /// <returns>Deletes Product</returns>
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


        /// <summary>
        /// Checks to see if the product exists already
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
