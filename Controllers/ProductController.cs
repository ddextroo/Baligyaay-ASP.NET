using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Baligyaay.Helpers;
using System.Data;
using System.Text.RegularExpressions;

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

        [HttpGet]
        public async Task<IActionResult> GetAllProduct(int categoryId = -1)
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
                        if (categoryId == -1)
                        {
                            command.CommandText = @"SELECT product.prod_id, product.prod_name, product.prod_description, product.prod_price, product.prod_stock, product.prod_img_url, 
                                                category.cat_id, category.cat_name, product_char.char_id, product_char.char_material, product_char.char_length, 
                                                product_char.char_width, product_char.char_height, product_char.char_weight 
                                            FROM product 
                                            INNER JOIN category ON product.cat_id = category.cat_id 
                                            INNER JOIN product_char ON product.prod_id = product_char.prod_id";
                        }
                        else
                        {
                            command.CommandText = @"SELECT product.prod_id, product.prod_name, product.prod_description, product.prod_price, product.prod_stock, product.prod_img_url, 
                                                category.cat_id, category.cat_name, product_char.char_id, product_char.char_material, product_char.char_length, 
                                                product_char.char_width, product_char.char_height, product_char.char_weight 
                                            FROM product 
                                            INNER JOIN category ON product.cat_id = category.cat_id 
                                            INNER JOIN product_char ON product.prod_id = product_char.prod_id
                                            WHERE category.cat_id = @categoryId";
                            command.Parameters.AddWithValue("@categoryId", categoryId);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var products = new List<JoinedProduct>();
                            while (await reader.ReadAsync())
                            {
                                var product = new JoinedProduct
                                {
                                    prod_id = reader.GetInt32("prod_id"),
                                    prod_name = reader.GetString("prod_name"),
                                    prod_description = reader.GetString("prod_description"),
                                    prod_price = reader.GetDecimal("prod_price"),
                                    prod_stock = reader.GetInt32("prod_stock"),
                                    prod_img_url = reader.GetString("prod_img_url"),
                                    cat_id = reader.GetInt32("cat_id"),
                                    cat_name = reader.GetString("cat_name"),
                                    char_id = reader.GetInt32("char_id"),
                                    char_material = reader.GetString("char_material"),
                                    char_length = reader.GetDecimal("char_length"),
                                    char_width = reader.GetDecimal("char_width"),
                                    char_height = reader.GetDecimal("char_height"),
                                    char_weight = reader.GetDecimal("char_weight")
                                };
                                products.Add(product);
                            }

                            return Ok(products);
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

        [HttpDelete("Delete/{prodId}")]
        public async Task<IActionResult> DeleteOrderItem(int prodId)

        {
            try
            {
                await InitializeAsync();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM product WHERE prod_id = @prodId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@prodId", prodId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return Ok("Product deleted successfullyw");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("Category/Delete/{catId}")]
        public async Task<IActionResult> DeleteCategory(int catId)
        {
            try
            {
                await InitializeAsync();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM category WHERE cat_id = @catId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@catId", catId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok("Category deleted successfully");
                        }
                        else
                        {
                            return NotFound("Category not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }


        [HttpGet("GetProductById/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                await InitializeAsync();

                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(@"
                SELECT product.prod_id, product.prod_name, product.prod_description, product.prod_price, product.prod_stock, product.prod_img_url, 
                       category.cat_id, category.cat_name, product_char.char_id, product_char.char_material, product_char.char_length, 
                       product_char.char_width, product_char.char_height, product_char.char_weight 
                FROM product 
                INNER JOIN category ON product.cat_id = category.cat_id 
                INNER JOIN product_char ON product.prod_id = product_char.prod_id
                WHERE product.prod_id = @productId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@productId", productId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var product = new JoinedProduct
                                {
                                    prod_id = reader.GetInt32("prod_id"),
                                    prod_name = reader.GetString("prod_name"),
                                    prod_description = reader.GetString("prod_description"),
                                    prod_price = reader.GetDecimal("prod_price"),
                                    prod_stock = reader.GetInt32("prod_stock"),
                                    prod_img_url = reader.GetString("prod_img_url"),
                                    cat_id = reader.GetInt32("cat_id"),
                                    cat_name = reader.GetString("cat_name"),
                                    char_id = reader.GetInt32("char_id"),
                                    char_material = reader.GetString("char_material"),
                                    char_length = reader.GetDecimal("char_length"),
                                    char_width = reader.GetDecimal("char_width"),
                                    char_height = reader.GetDecimal("char_height"),
                                    char_weight = reader.GetDecimal("char_weight")
                                };

                                return Ok(product);
                            }
                            else
                            {
                                return NotFound("Product not found");
                            }
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
        [HttpPatch("UpdateCategory/{catId}")]
        public async Task<IActionResult> UpdateCategory(int catId, [FromBody] Category updatedCategory)
        {
            try
            {
                await InitializeAsync();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("UPDATE category SET cat_name = @name WHERE cat_id = @catId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@name", updatedCategory.name);
                        command.Parameters.AddWithValue("@catId", catId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok("Category updated successfully");
                        }
                        else
                        {
                            return NotFound("Category not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpGet("GetCategoryById/{catId}")]
        public async Task<IActionResult> GetCategoryById(int catId)
        {
            try
            {
                await InitializeAsync();

                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(@"
                SELECT cat_id, cat_name 
                FROM category 
                WHERE cat_id = @catId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@catId", catId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var category = new
                                {
                                    cat_id = reader.GetInt32("cat_id"),
                                    cat_name = reader.GetString("cat_name")
                                };

                                return Ok(category);
                            }
                            else
                            {
                                return NotFound("Category not found");
                            }
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

                    var base64Data = Regex.Match(productCreationDto.Product.image, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                    var imageData = Convert.FromBase64String(base64Data);
                    var filename = Guid.NewGuid() + ".png";
                    var filePath = Path.Combine("wwwroot", "uploads", $"{filename}");
                    System.IO.File.WriteAllBytes(filePath, imageData);

                    // Insert into the product table with OUTPUT clause
                    using (var command = new SqlCommand("INSERT INTO product (prod_name, prod_description, prod_price, prod_stock, prod_img_url, cat_id) OUTPUT INSERTED.prod_id VALUES (@name, @description, @price, @stock, @image, @cat_id)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@name", productCreationDto.Product.name);
                        command.Parameters.AddWithValue("@description", productCreationDto.Product.description);
                        command.Parameters.AddWithValue("@price", productCreationDto.Product.price);
                        command.Parameters.AddWithValue("@stock", productCreationDto.Product.stock);
                        command.Parameters.AddWithValue("@image", "/uploads/" + filename);
                        command.Parameters.AddWithValue("@cat_id", productCreationDto.Product.category);

                        int productId = (int)command.ExecuteScalar();

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
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return StatusCode(500, ex.Message); // Consider returning a more user-friendly error message
            }
        }

        [HttpPatch("UpdateProduct/{productId}")]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductCreationDTO productUpdateDto)
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

                    string filename = null;
                    if (productUpdateDto.Product.image != "wala")
                    {
                        var base64Data = Regex.Match(productUpdateDto.Product.image, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                        var imageData = Convert.FromBase64String(base64Data);
                        filename = Guid.NewGuid() + ".png";
                        var filePath = Path.Combine("wwwroot", "uploads", $"{filename}");
                        System.IO.File.WriteAllBytes(filePath, imageData);
                    }

                    string updateProductQuery = @"
                UPDATE product 
                SET 
                    prod_name = @name, 
                    prod_description = @description, 
                    prod_price = @price, 
                    prod_stock = @stock, 
                    cat_id = @cat_id ";

                    if (filename != null)
                    {
                        updateProductQuery += ", prod_img_url = @image ";
                    }

                    updateProductQuery += "WHERE prod_id = @productId";

                    using (var command = new SqlCommand(updateProductQuery, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@name", productUpdateDto.Product.name);
                        command.Parameters.AddWithValue("@description", productUpdateDto.Product.description);
                        command.Parameters.AddWithValue("@price", productUpdateDto.Product.price);
                        command.Parameters.AddWithValue("@stock", productUpdateDto.Product.stock);
                        command.Parameters.AddWithValue("@cat_id", productUpdateDto.Product.category);
                        command.Parameters.AddWithValue("@productId", productId);

                        if (filename != null)
                        {
                            command.Parameters.AddWithValue("@image", "/uploads/" + filename);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected <= 0)
                        {
                            return NotFound("Product not found");
                        }
                    }

                    using (var command2 = new SqlCommand(@"
                UPDATE product_char 
                SET 
                    char_material = @material, 
                    char_length = @length, 
                    char_width = @width, 
                    char_height = @height, 
                    char_weight = @weight 
                WHERE prod_id = @productId", connection))
                    {
                        command2.CommandType = CommandType.Text;
                        command2.Parameters.AddWithValue("@material", productUpdateDto.ProductChar.material);
                        command2.Parameters.AddWithValue("@length", productUpdateDto.ProductChar.length);
                        command2.Parameters.AddWithValue("@width", productUpdateDto.ProductChar.width);
                        command2.Parameters.AddWithValue("@height", productUpdateDto.ProductChar.height);
                        command2.Parameters.AddWithValue("@weight", productUpdateDto.ProductChar.weight);
                        command2.Parameters.AddWithValue("@productId", productId);

                        int charRowsAffected = command2.ExecuteNonQuery();
                        if (charRowsAffected <= 0)
                        {
                            return StatusCode(500, "Failed to update product characteristics");
                        }
                    }

                    return Ok("Product updated successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("UpdateStock/{prodId}/{newOrderStock}")]
        public async Task<IActionResult> UpdateStock(int prodId, int newOrderStock)
        {
            try
            {
                await InitializeAsync();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("UPDATE product SET prod_stock = @newOrderStock WHERE prod_id = @prodId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@newOrderStock", newOrderStock);
                        command.Parameters.AddWithValue("@prodId", prodId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok("Stock updated from product");
                        }
                        else
                        {
                            return NotFound("Order item not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here using a logging framework, e.g., Serilog, NLog, etc.
                return StatusCode(500, ex);
            }
        }


    }


}