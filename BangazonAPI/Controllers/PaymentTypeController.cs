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
    [Route("api/[controller]s")]
    [ApiController]
    public class PaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
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
        /// Gets all Payment Types from the database
        /// </summary>
        /// <returns>List of Payment Types</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Active FROM PaymentType";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<PaymentType> paymentTypes = new List<PaymentType>();

                    while (reader.Read())
                    {
                        PaymentType paymentType = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active"))
                        };
                        if (paymentType.Active == true)
                        {
                            paymentTypes.Add(paymentType);
                        }
                    }
                    reader.Close();

                    return Ok(paymentTypes);
                }
            }
        }


        /// <summary>
        /// Get a single payment type by Id
        /// </summary>
        /// <param name="id">Id of payment type to display</param>
        /// <returns>A single payment type</returns>
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Name, Active
                        FROM PaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    PaymentType paymentType = null;

                    if (reader.Read())
                    {
                        paymentType = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active"))
                        };
                    }
                    else
                    {
                        return NotFound();
                    }
                    reader.Close();

                    return Ok(paymentType);
                }
            }
        }


        /// <summary>
        /// Create a new Payment Type
        /// </summary>
        /// <param name="paymentType">Creates a new Payment Type with provided body values \
        /// Must include : \
        /// Name: string \
        /// Active: boolean
        ///</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType paymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO PaymentType (Name, Active)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @Active)";
                    cmd.Parameters.Add(new SqlParameter("@Name", paymentType.Name));
                    cmd.Parameters.Add(new SqlParameter("@Active", paymentType.Active));

                    int newId = (int)cmd.ExecuteScalar();
                    paymentType.Id = newId;
                    return CreatedAtRoute("GetPaymentType", new { id = newId }, paymentType);
                }
            }
        }

        /// <summary>
        /// Changes the active property of the selected computer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paymentType"> \
        /// Must include : \
        /// Name: string \
        /// Active: boolean</param> 
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentType paymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE PaymentType
                                            SET Name = @Name,
                                                Active = @Active
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Name", paymentType.Name));
                        cmd.Parameters.Add(new SqlParameter("@Active", paymentType.Active));
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
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool PaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Active
                        FROM PaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}