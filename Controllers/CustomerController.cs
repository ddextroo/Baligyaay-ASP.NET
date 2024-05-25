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
public class CustomerController : ControllerBase
{
    private readonly SessionManager _sessionManager;

    private readonly IConfiguration _configuration;
    private bool isConnected;

    public CustomerController(IConfiguration configuration, SessionManager sessionManager)
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
                    command.CommandText = "SELECT * FROM customer";

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


    // GET: api/Customer/5
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok("HAHAHA");
    }

    // POST: api/Customer
    [HttpPost]
    public IActionResult Create([FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Validate individual properties
            if (string.IsNullOrEmpty(customer.FirstName))
            {
                return BadRequest("First name is required.");
            }

            if (EmailExists(customer.Email))
            {
                return Conflict("Email already exists.");
            }

            if (string.IsNullOrEmpty(customer.LastName))
            {
                return BadRequest("Last name is required.");
            }

            if (string.IsNullOrEmpty(customer.Email) || !new EmailAddressAttribute().IsValid(customer.Email))
            {
                return BadRequest("A valid email is required.");
            }

            if (!string.IsNullOrEmpty(customer.Phone))
            {
                if (!Regex.IsMatch(customer.Phone, @"^09\d{9}$"))
                {
                    return BadRequest("A valid phone number is required. It should start with 09 and have a total of 11 digits.");
                }
            }

            if (string.IsNullOrEmpty(customer.Password) || customer.Password.Length < 6)
            {
                return BadRequest("Password is required and must be at least 6 characters long.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            customer.Password = hashedPassword;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO customer (cus_fname, cus_lname, cus_email, cus_phone, cus_password) VALUES (@fname, @lname, @mail, @phone, @pass)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fname", customer.FirstName);
                    command.Parameters.AddWithValue("@lname", customer.LastName);
                    command.Parameters.AddWithValue("@mail", customer.Email);
                    command.Parameters.AddWithValue("@phone", (object)customer.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@pass", customer.Password);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return Ok("Customer created successfully");
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


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (loginModel == null)
        {
            return BadRequest("Invalid client request");
        }

        try
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM customer WHERE cus_email = @Email";
                    command.Parameters.AddWithValue("@Email", loginModel.Email);

                    var reader = await command.ExecuteReaderAsync();
                    if (!reader.Read())
                    {
                        return StatusCode(403, "Customer not registered");
                    }

                    string storedPasswordHash = reader.GetString(reader.GetOrdinal("cus_password"));

                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginModel.Password, storedPasswordHash);

                    if (!isPasswordValid)
                    {
                        return StatusCode(403, "Invalid password");
                    }

                    _sessionManager.SetSessionValue("Email", loginModel.Email);
                    return Ok("Customer Login successfully");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        try
        {
            var email = _sessionManager.GetSessionValue("Email");

            if (email != null)
            {
                _sessionManager.RemoveSessionValue("Email");
                return Ok("User logout successfully");
            }
            return BadRequest("User not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPost("AdminLogout")]
    public IActionResult AdminLogout()
    {
        try
        {
            var admin = _sessionManager.GetSessionValue("admin");

            if (admin != null)
            {
                _sessionManager.RemoveSessionValue("admin");
                return Ok("Admin logout successfully");
            }
            return BadRequest("User not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }




    // PUT: api/Customer/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Customer customer)
    {
        return Ok("HAHAHA");
    }

    // DELETE: api/Customer/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok("HAHAHA");
    }

    private bool EmailExists(string email)
    {
        try
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM customer WHERE cus_email = @Email", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Email", email);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (implementation of logging is not shown here)
            throw new Exception("An error occurred while checking the email", ex);
        }
    }

    [HttpGet("getcustomer")]
    public async Task<IActionResult> GetCustomerDetails(string emailAddress)
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

                    command.CommandText = @"SELECT * FROM customer WHERE cus_email = @emailAddress";
                    command.Parameters.AddWithValue("@emailAddress", emailAddress);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var customers = new List<Customer>();
                        while (await reader.ReadAsync())
                        {
                            var customer = new Customer
                            {
                                Id = reader.GetInt32("cus_id"),
                                FirstName = reader.GetString("cus_fname"),
                                LastName = reader.GetString("cus_lname"),
                                Phone = reader.GetString("cus_phone"),
                                Email = reader.GetString("cus_email"),
                                Password = reader.GetString("cus_password"),

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

            var json = System.Text.Json.JsonSerializer.Serialize(serializedException);

            return StatusCode(500, json);
        }
    }

    [HttpDelete("Delete/{cusId}")]
    public async Task<IActionResult> DeleteCustomer(int cusId)
    {
        try
        {
            await InitializeAsync();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DELETE FROM customer WHERE cus_id = @cusId", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@cusId", cusId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return Ok("Customer deleted successfully");
                    }
                    else
                    {
                        return NotFound("Customer not found");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

}


/*

Ok(data): Returns a 200 OK response with the data.
NotFound(): Returns a 404 Not Found response.
CreatedAtAction(): Returns a 201 Created response with the new item.
NoContent(): Returns a 204 No Content response.
*/