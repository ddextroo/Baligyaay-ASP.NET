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
    public class OrderController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        private readonly IConfiguration _configuration;
        private bool isConnected;

        public OrderController(IConfiguration configuration, SessionManager sessionManager)
        {
            _configuration = configuration;
            _sessionManager = sessionManager;
        }
        private async Task InitializeAsync()
        {
            isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn")!);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] CustomerOrder customerOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    connection.Open();
                    using (var command = new SqlCommand("INSERT INTO order_items (order_item_quantity, order_item_price, cus_id, prod_id) VALUES (@quantity, @price, @cus_id, @prod_id)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@quantity", customerOrder.OrderItemQuantity);
                        command.Parameters.AddWithValue("@price", customerOrder.OrderItemPrice);
                        command.Parameters.AddWithValue("@cus_id", customerOrder.CusId);
                        command.Parameters.AddWithValue("@prod_id", customerOrder.ProdId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok("Item added to cart");
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

        [HttpDelete("DeleteOrder/{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItem(int orderItemId)

        {
            try
            {
                await InitializeAsync();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM order_items WHERE order_item_id = @orderItemId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@orderItemId", orderItemId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok("Item deleted from cart");
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
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        [HttpPut("UpdateOrder/{orderId}/{orderQuantity}")]
        public async Task<IActionResult> UpdateQuantity(int orderId, int orderQuantity)
        {
            try
            {
                await InitializeAsync();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("baligyaayconn")!))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("UPDATE order_items SET order_item_quantity = @orderQuantity WHERE order_item_id = @orderId", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@orderQuantity", orderQuantity);
                        command.Parameters.AddWithValue("@orderId", orderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok("Quantity updated from cart");
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


        [HttpGet("getorders")]
        public async Task<IActionResult> GetCustomerOrders(string cus_id)
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

                        command.CommandText = @"
SELECT  
    order_items.order_item_id,
    order_items.order_item_quantity,
    order_items.order_item_price,
    customer.cus_id,
    customer.cus_fname,
    customer.cus_lname,
    customer.cus_email,
    customer.cus_phone,
    product.prod_id,
    product.prod_name,
    product.prod_description,
    product.prod_price,
    product.prod_stock,
    product.prod_img_url,
    category.cat_id,
    category.cat_name,
    product_char.char_id,
    product_char.char_material,
    product_char.char_length,
    product_char.char_width,
    product_char.char_height,
    product_char.char_weight
FROM
    order_items
    INNER JOIN customer ON customer.cus_id = order_items.cus_id
    INNER JOIN product ON product.prod_id = order_items.prod_id
    INNER JOIN category ON category.cat_id = product.cat_id
    INNER JOIN product_char ON product_char.prod_id = product.prod_id
    WHERE customer.cus_id = @cus_id";
                        command.Parameters.AddWithValue("@cus_id", cus_id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var orders = new List<CustomerAfterOrder>();
                            while (await reader.ReadAsync())
                            {
                                var order = new CustomerAfterOrder
                                {
                                    // OrderItem properties
                                    OrderItemId = reader.GetInt32(reader.GetOrdinal("order_item_id")),
                                    OrderItemQuantity = reader.GetInt32(reader.GetOrdinal("order_item_quantity")),
                                    OrderItemPrice = reader.GetDecimal(reader.GetOrdinal("order_item_price")),

                                    // Customer properties
                                    CusId = reader.GetInt32(reader.GetOrdinal("cus_id")),
                                    CusFname = reader.GetString(reader.GetOrdinal("cus_fname")),
                                    CusLname = reader.GetString(reader.GetOrdinal("cus_lname")),
                                    CusEmail = reader.GetString(reader.GetOrdinal("cus_email")),
                                    CusPhone = reader.GetString(reader.GetOrdinal("cus_phone")),

                                    // Product properties
                                    ProdId = reader.GetInt32(reader.GetOrdinal("prod_id")),
                                    ProdName = reader.GetString(reader.GetOrdinal("prod_name")),
                                    ProdDescription = reader.GetString(reader.GetOrdinal("prod_description")),
                                    ProdPrice = reader.GetDecimal(reader.GetOrdinal("prod_price")),
                                    ProdStock = reader.GetInt32(reader.GetOrdinal("prod_stock")),
                                    ProdImgUrl = reader.GetString(reader.GetOrdinal("prod_img_url")),

                                    // Category properties
                                    CatId = reader.GetInt32(reader.GetOrdinal("cat_id")),
                                    CatName = reader.GetString(reader.GetOrdinal("cat_name")),

                                    // ProductChar properties
                                    CharId = reader.GetInt32(reader.GetOrdinal("char_id")),
                                    CharMaterial = reader.GetString(reader.GetOrdinal("char_material")),
                                    CharLength = reader.GetDecimal(reader.GetOrdinal("char_length")),
                                    CharWidth = reader.GetDecimal(reader.GetOrdinal("char_width")),
                                    CharHeight = reader.GetDecimal(reader.GetOrdinal("char_height")),
                                    CharWeight = reader.GetDecimal(reader.GetOrdinal("char_weight"))

                                };
                                orders.Add(order);
                            }

                            return Ok(orders);
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
    }
}