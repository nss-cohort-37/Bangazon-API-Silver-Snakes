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
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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
        /// Get all Training Programs
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
                    cmd.CommandText = @"SELECT Id, Name, StartDate,  EndDate, MaxAttendees
                                        FROM TrainingProgram";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();

                    return Ok(trainingPrograms);
                }
            }
        }
        /// <summary>
        /// View a specific training program by id
        /// </summary>
        /// <param name="id"> id of traingin program you want to view</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, StartDate, EndDate, MaxAttendees
                                        FROM TrainingProgram where Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };

                        }

                    }

                     reader.Close();

                    return Ok(trainingProgram);
                }
            }
        }


        /// <summary>
        /// Add an employee to a training program
        /// </summary>
        /// <param name="trainingProgramId">id of program that the employee is being added to</param>
        /// <param name="employeeTrainingProgram">
        /// trainingProgramId: int [From Route] \
        /// employeeId: int (Id of employee you are adding)</param>
        /// <returns></returns>
        [HttpPost("{trainingProgramId}/employees")]
        public async Task<IActionResult> Post([FromRoute] int trainingProgramId ,[FromBody] EmployeeTrainingProgram employeeTrainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO EmployeeTraining ( EmployeeId, TrainingProgramId  )
                                        OUTPUT INSERTED.Id
                                        VALUES (@EmployeeId, @TrainingProgramId)";
                    cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeTrainingProgram.EmployeeId));
                    cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", employeeTrainingProgram.TrainingProgramId));
                    int newId = (int)cmd.ExecuteScalar();
                    employeeTrainingProgram.Id = newId;
                    return CreatedAtRoute("GetTrainingProgram", new { id = newId }, employeeTrainingProgram);
                }
            }
        }


        /// <summary>
        /// Add a training program
        /// </summary>
        /// <param name="trainingProgram">Must include: \
        /// name: string \
        /// startDate: datetime \
        /// endDate: dateTime \
        /// maxAttendees: int 
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO TrainingProgram ( Name, StartDate, EndDate, MaxAttendees )
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @StartDate, @EndDate, @MaxAttendees)";
                    cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));


                    int newId = (int)cmd.ExecuteScalar();
                    trainingProgram.Id = newId;
                    return CreatedAtRoute("GetTrainingProgram", new { id = newId }, trainingProgram);
                }
            }
        }


        /// <summary>
        /// Edit a training program
        /// </summary>
        /// <param name="id">Id of program you want to edit</param>
        /// <param name="trainingProgram">Must include: \
        /// id: int [FromRoute] \
        /// name: string \
        /// startDate: datetime \
        /// endDate: dateTime \
        /// maxAttendees: int </param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @Name,
                                                StartDate = @StartDate,  
                                                EndDate = @EndDate, 
                                                MaxAttendees = @Maxattendees                                           
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Id", trainingProgram.Id));
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));


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
                if (!TrainingProgramExists(id))
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
        /// Delete training program
        /// </summary>
        /// <param name="id"> Id of training program you want to delete</param>
        /// <param name="trainingProgram"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"Delete from TrainingProgram
                                           
                                            WHERE Id = @id";

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
                if (!TrainingProgramExists(id))
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
        /// Remove an employee from a training program
        /// </summary>
        /// <param name="trainingProgramId"> Id of training program</param>
        /// <param name="employeeId"> Id of employee</param>
        /// <returns></returns>
        [HttpDelete("{trainingProgramId}/employees/{employeeId}")]
        public async Task<IActionResult> Delete([FromRoute] int trainingProgramId, [FromRoute] int employeeId)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        


                        
                            cmd.CommandText = @"Delete from EmployeeTraining
                                           
                                            WHERE TrainingProgramId = @trainingProgramId AND EmployeeId = @employeeId";

                            cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                            cmd.Parameters.Add(new SqlParameter("@trainingProgramId", trainingProgramId));

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
                if (!TrainingProgramExists(trainingProgramId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }









        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, StartDate, EndDate, MaxAttendees
                                        FROM TrainingProgram where Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}