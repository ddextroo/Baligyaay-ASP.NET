using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Baligyaay.Models;
using Microsoft.AspNetCore.Mvc;

namespace Baligyaay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private static class DatabaseHelper
        {
            public static async Task<bool> IsServerConnected(string connectionString)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        await connection.OpenAsync();
                        return true;
                    }
                    catch (SqlException)
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<IActionResult> IndexAsync()
        {
            bool isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn")!);
            ViewBag.ConnectionStatus = isConnected ? "Connected" : "Not Connected";
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Admin()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
