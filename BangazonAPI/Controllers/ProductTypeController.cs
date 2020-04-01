//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using BangazonAPI.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;

//namespace BangazonAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductTypeController : ControllerBase
//    {
//        private readonly IConfiguration _config;

//        public ProductTypeController(IConfiguration config)
//        {
//            _config = config;
//        }

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = "SELECT Id, Name FROM ProductType";
//                    SqlDataReader reader = cmd.ExecuteReader();
//                    List<ProductType> customers = new List<ProductType>();

//                    while (reader.Read())
//                    {
//                        ProductType customer = new ProductType
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            Name = reader.GetString(reader.GetOrdinal("Name")),

//                        };
//                    }
//                }
//            }
//        }
//    }
//}