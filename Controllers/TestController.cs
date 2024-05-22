using Microsoft.AspNetCore.Mvc;
using Baligyaay.Models;
using System.Data.SqlClient;
using Baligyaay.Helpers;
using System.Data;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly SessionManager _sessionManager;

    private readonly IConfiguration _configuration;
    private bool isConnected;

    public TestController(IConfiguration configuration, SessionManager sessionManager)
    {
        _configuration = configuration;
        _sessionManager = sessionManager;
    }

    private async Task InitializeAsync()
    {
        isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn")!);
    }

    // GET: api/Customer
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
                    command.CommandText = "SELECT * FROM test";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var tests = new List<Test>();
                        while (await reader.ReadAsync())
                        {
                            var test = new Test
                            {
                                id = reader.GetInt32("test_id"),
                                name = reader["test_name"].ToString()!,
                                img = reader["test_img"].ToString()!,
                            };
                            tests.Add(test);
                        }

                        return Ok(tests);
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

    [HttpPost]
    public IActionResult Create([FromBody] Test test)
    {
        try
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO test (test_name, test_img) VALUES (@name, @img)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@name", test.name);
                    command.Parameters.AddWithValue("@img", test.img);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return Ok("Test created successfully");
                    }
                    else
                    {
                        return StatusCode(500, "Failed to insert test");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (implementation of logging is not shown here)
            return StatusCode(500, ex);
        }
    }

}


/*

Ok(data): Returns a 200 OK response with the data.
NotFound(): Returns a 404 Not Found response.
CreatedAtAction(): Returns a 201 Created response with the new item.
NoContent(): Returns a 204 No Content response.
*/