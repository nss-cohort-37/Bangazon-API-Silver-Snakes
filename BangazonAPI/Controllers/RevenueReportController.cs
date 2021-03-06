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
    public class RevenueReportController : ControllerBase
    {
        private readonly IConfiguration _config;

        public RevenueReportController(IConfiguration config)
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
        /// View revenue report for each product type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select pt.[Name], pt.Id, SUM(p.Price) as Revenue
                    from OrderProduct op left join Product p on p.Id = op.ProductId
                    full outer join ProductType pt on p.ProductTypeId = pt.Id group by pt.[Name], pt.Id ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<RevenueReport> revenueReport = new List<RevenueReport>();
                    RevenueReport reportItem = null;
                    while (reader.Read())
                    {
                        


                        reportItem = new RevenueReport
                        {
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductType = reader.GetString(reader.GetOrdinal("Name"))
                            
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Revenue")))
                        {
                            reportItem.TotalRevenue = reader.GetDecimal(reader.GetOrdinal("Revenue"));
                        }
                        else
                        {
                            reportItem.TotalRevenue = 0;
                        }

                        revenueReport.Add(reportItem);
                    }
                    reader.Close();

                    return Ok(revenueReport);
                }
            }
        }



    }
}