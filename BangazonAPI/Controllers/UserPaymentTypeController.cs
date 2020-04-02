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
    public class UserPaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;
        public UserPaymentTypeController(IConfiguration config)
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
        //[HttpGet]

        //public async Task<IActionResult> Get()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId, AcctNumber, Active
        //                FROM UserPaymentType";

        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<UserPaymentType> userPaymentTypes = new List<UserPaymentType>();

        //            while (reader.Read())
        //            {
        //                UserPaymentType userPaymentType = new UserPaymentType
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
        //                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
        //                    AcctNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
        //                    Active = reader.GetBoolean(reader.GetOrdinal("Active")),
        //                };

        //                userPaymentTypes.Add(userPaymentType);

        //            }
        //            reader.Close();

        //            return Ok(userPaymentTypes);
        //        }

        //    }
        //}


        //Get User Payment Types
        [HttpGet]

        public async Task<IActionResult> Get([FromQuery] Int32? customerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT upt.Id, upt.CustomerId, upt.PaymentTypeId, upt.AcctNumber, upt.active
                        FROM UserPaymentType upt
                        WHERE upt.CustomerId = @customerId";

                    cmd.Parameters.Add(new SqlParameter("@customerId", customerId));
                    SqlDataReader reader = cmd.ExecuteReader();
                    UserPaymentType userPaymentType = null;
                    List<UserPaymentType> userPaymentTypes = new List<UserPaymentType>();

                    while (reader.Read())
                    {
                    userPaymentType = new UserPaymentType
                    {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                            AcctNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                        };
                        userPaymentTypes.Add(userPaymentType);
                    }
                    reader.Close();

                    return Ok(userPaymentTypes);
                }

            }
        }
        //POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserPaymentType userPaymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO UserPaymentType (CustomerId, PaymentTypeId, AcctNumber, Active)
                                        OUTPUT INSERTED.Id
                                        VALUES (@customerId, @paymentTypeId, @acctNumber, @active)";
                    cmd.Parameters.Add(new SqlParameter("@customerId", userPaymentType.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@paymentTypeId", userPaymentType.PaymentTypeId));
                    cmd.Parameters.Add(new SqlParameter("@acctNumber", userPaymentType.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@active", userPaymentType.Active));

                    int newId = (int)cmd.ExecuteScalar();
                    userPaymentType.Id = newId;
                    return CreatedAtRoute("GetUserPaymentType", new { id = newId }, userPaymentType);
                }
            }
        }

        // Put
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] UserPaymentType userPaymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE UserPaymentType
                                            SET CustomerId = @customerId,
                                            PaymentTypeId = @paymentTypeId,
                                            AcctNumber = @acctNumber,
                                            Active = @active
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@customerId", userPaymentType.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", userPaymentType.PaymentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@acctNumber", userPaymentType.AcctNumber));
                        cmd.Parameters.Add(new SqlParameter("@active", userPaymentType.Active));
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
                if (!UserPaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //Soft "Delete" a paymentType
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromBody] UserPaymentType userPaymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE UserPaymentType
                                            SET Active = @Active
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@active", userPaymentType.Active));
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
                if (!UserPaymentTypeExists(id))
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
        private bool UserPaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, CustomerId, PaymentTypeId, AcctNumber, Active
                        FROM UserPaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}