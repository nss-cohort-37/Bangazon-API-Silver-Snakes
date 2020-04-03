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
    public class ComputerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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

        //Get all computers from the database
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool available)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, PurchaseDate, DecomissionDate, Make, Model FROM Computer";

                    if (available == false)
                    {
                        cmd.CommandText += " WHERE DecomissionDate IS NOT NULL OR Id = ANY (SELECT ComputerId FROM Employee)";
                    }
                    if (available == true)
                    {
                        cmd.CommandText += " WHERE DecomissionDate IS NULL AND Id NOT IN (SELECT ComputerId FROM Employee)";
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();



                    while (reader.Read())
                    {
                        Computer computer = null;


                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                 
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))
                            };
                        }
                        else
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))
                            };
                        }

                        computers.Add(computer);
                    }
                    reader.Close();

                    return Ok(computers);
                }
            }
        }

        //////Get unavailable computers
        //[HttpGet]
        
        //public async Task<IActionResult> Get()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Model
        //                                FROM Computer 
        //                                WHERE DecomissionDate IS NOT NULL OR Id = ANY (SELECT ComputerId FROM Employee)
        //                                ";
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<Computer> usedComputers = new List<Computer>();

        //            while (reader.Read())
        //            {
        //                Computer computer = null;

        //                if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
        //                {
        //                    computer = new Computer
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
        //                        DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
        //                        Make = reader.GetString(reader.GetOrdinal("Make")),
        //                        Model = reader.GetString(reader.GetOrdinal("Model"))
        //                    };
        //                }
        //                else
        //                {
        //                    computer = new Computer
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
        //                        Make = reader.GetString(reader.GetOrdinal("Make")),
        //                        Model = reader.GetString(reader.GetOrdinal("Model"))
        //                    };
        //                }
        //                usedComputers.Add(computer);
        //                    available = false;
        //                }
        //            reader.Close();

        //            return Ok(usedComputers);
        //        }
        //    }
        //}


        //Get a single computer by Id
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, PurchaseDate, DecomissionDate, Make, Model
                        FROM Computer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;

                    if (reader.Read())
                    { 

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))
                            };
                        }
                        else
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))
                            };
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                    reader.Close();

                    return Ok(computer);
                }
            }
        }


        //Add a new computer
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                   
                     cmd.CommandText = @"INSERT INTO Computer (PurchaseDate, Make, Model)
                                        OUTPUT INSERTED.Id
                                        VALUES (@PurchaseDate, @Make, @Model)";

                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", DateTime.Now));
                        cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Model", computer.Model));
                    


                    int newId = (int)cmd.ExecuteScalar();
                    computer.PurchaseDate = DateTime.Now;
                    computer.Id = newId;
                    return CreatedAtRoute("GetComputer", new { id = newId }, computer);
                }
            }
        }

        //Update a computer record
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Computer computer)
        {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE Computer
                                            SET PurchaseDate = @PurchaseDate,
                                                DecomissionDate = @DecomissionDate,
                                                Make = @Make,
                                                Model = @Model
                                            WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                            cmd.Parameters.Add(new SqlParameter("@DecomissionDate", computer.DecomissionDate));
                            cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@Model", computer.Model));
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
                    if (!ComputerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
        }


        //Delete a computer record
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
                        cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";
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
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ComputerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, PurchaseDate, DecomissionDate, Make, Model
                        FROM Computer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}