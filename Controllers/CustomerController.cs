using Microsoft.AspNetCore.Mvc;
using Baligyaay.Models;
using System.Data.SqlClient;
using Baligyaay.Helpers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private bool isConnected;

    public CustomerController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private async Task InitializeAsync()
    {
        isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn")!);
    }

    // GET: api/Customer
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!isConnected)
        {
            await InitializeAsync();
        }

        // Rest of your action method logic...
        return Ok(new { message = 1, isConnected = isConnected });
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
        return Ok("HAHAHA");
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
}


/*

Ok(data): Returns a 200 OK response with the data.
NotFound(): Returns a 404 Not Found response.
CreatedAtAction(): Returns a 201 Created response with the new item.
NoContent(): Returns a 204 No Content response.
*/