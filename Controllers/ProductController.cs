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
        [HttpGet("Category")]
        public async Task<IActionResult> GetAllCategory()
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
                        command.CommandText = "SELECT * FROM category ";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var categories = new List<Category>();
                            while (await reader.ReadAsync())
                            {
                                var category = new Category
                                {
                                    id = reader.GetInt32("cat_id"),
                                    name = reader["cat_name"].ToString(),
                                };
                                categories.Add(category);
                            }

                            return Ok(categories);
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

        [HttpPost("Category")]
        public IActionResult CreateCategory([FromBody] Category category)
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

        [HttpPost]
        public IActionResult Create([FromBody] ProductCreationDTO productCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")))
                {
                    connection.Open();

                    // Get the image file name
                    string imageName = productCreationDto.Product.image;

                    // Save the image path
                    string imagePath = Path.Combine("Image", imageName); // Path to save in Image directory
                    string absoluteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath); // Absolute path

                    // Insert into the product table with OUTPUT clause
                    using (var command = new SqlCommand("INSERT INTO product (prod_name, prod_description, prod_price, prod_stock, prod_img_url, cat_id) OUTPUT INSERTED.prod_id VALUES (@name, @description, @price, @stock, @image, @cat_id)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@name", productCreationDto.Product.name);
                        command.Parameters.AddWithValue("@description", productCreationDto.Product.description);
                        command.Parameters.AddWithValue("@price", productCreationDto.Product.price);
                        command.Parameters.AddWithValue("@stock", productCreationDto.Product.stock);
                        command.Parameters.AddWithValue("@image", imagePath);
                        command.Parameters.AddWithValue("@cat_id", productCreationDto.Product.category);

                        int productId = (int)command.ExecuteScalar();

                        // Insert into the product_char table
                        using (var command2 = new SqlCommand("INSERT INTO product_char (char_material, char_length, char_width, char_height, char_weight, prod_id) VALUES (@material, @length, @width, @height, @weight, @prod_id)", connection))
                        {
                            command2.CommandType = CommandType.Text;
                            command2.Parameters.AddWithValue("@material", productCreationDto.ProductChar.material);
                            command2.Parameters.AddWithValue("@length", productCreationDto.ProductChar.length);
                            command2.Parameters.AddWithValue("@width", productCreationDto.ProductChar.width);
                            command2.Parameters.AddWithValue("@height", productCreationDto.ProductChar.height);
                            command2.Parameters.AddWithValue("@weight", productCreationDto.ProductChar.weight);
                            command2.Parameters.AddWithValue("@prod_id", productId);

                            int charRowsAffected = command2.ExecuteNonQuery();
                            if (charRowsAffected <= 0)
                            {
                                return StatusCode(500, "Failed to insert product");
                            }
                        }

                        return Ok("Product Added Successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return StatusCode(500, ex.Message); // Consider returning a more user-friendly error message
            }
        }


    }


}