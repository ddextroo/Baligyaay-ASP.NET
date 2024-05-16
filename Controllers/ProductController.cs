using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Baligyaay.Helpers;
using System.Data;

namespace Baligyaay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        private readonly IConfiguration _configuration;
        private bool isConnected;

        public ProductController(IConfiguration configuration, SessionManager sessionManager)
        {
            _configuration = configuration;
            _sessionManager = sessionManager;
        }
        private async Task InitializeAsync()
        {
            isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn")!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                await InitializeAsync();

                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT * FROM product ";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var customers = new List<Customer>();
                            while (await reader.ReadAsync())
                            {
                                var customer = new Customer
                                {
                                    Id = reader.GetInt32("cus_id"),
                                    FirstName = reader["cus_fname"].ToString(),
                                    LastName = reader["cus_lname"].ToString(),
                                    Phone = reader["cus_phone"].ToString(),
                                    Email = reader["cus_email"].ToString(),
                                    Password = reader["cus_password"].ToString()
                                };
                                customers.Add(customer);
                            }

                            return Ok(customers);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var serializedException = new
                {
                    ex.Message,
                    ex.StackTrace,
                    ex.GetType().FullName
                };

                // Serialize the anonymous object to JSON
                var json = System.Text.Json.JsonSerializer.Serialize(serializedException);

                // Return the JSON representation of the exception
                return StatusCode(500, json);
            }
        }

        [HttpPost("Category")]
        public IActionResult Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validate individual properties
                if (string.IsNullOrEmpty(category.name))
                {
                    return BadRequest("Category name is required.");
                }

                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    connection.Open();
                    using (var command = new SqlCommand("INSERT INTO category (cat_name) VALUES (@name)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@name", category.name);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok("Category created successfully");
                        }
                        else
                        {
                            return StatusCode(500, "Failed to insert customer");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (implementation of logging is not shown here)
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }


}