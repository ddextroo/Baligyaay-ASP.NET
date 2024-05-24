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
        private readonly SessionManager _sessionManager;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, SessionManager sessionManager)
        {
            _logger = logger;
            _configuration = configuration;
            _sessionManager = sessionManager;
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
            var email = _sessionManager.GetSessionValue("Email");

            if (email == null)
            {
                // Redirect to Home only if Email is null, to avoid a loop ensure Home action does not redirect back here
                return RedirectToAction("Login");
            }

            bool isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn"));
            ViewBag.ConnectionStatus = isConnected ? "Connected" : "Not Connected";
            ViewBag.Email = email;

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
        public IActionResult AdminLogin()
        {
            return View();
        }
        public IActionResult CategoryAdmin()
        {
            return View();
        }
        public IActionResult CustomerAdmin()
        {
            return View();
        }
        public async Task<IActionResult> Order()
        {
            var email = _sessionManager.GetSessionValue("Email");

            if (email == null)
            {
                // Redirect to Home only if Email is null, to avoid a loop ensure Home action does not redirect back here
                return RedirectToAction("Login");
            }

            bool isConnected = await DatabaseHelper.IsServerConnected(_configuration.GetConnectionString("baligyaayconn"));
            ViewBag.ConnectionStatus = isConnected ? "Connected" : "Not Connected";
            ViewBag.Email = email;

            return View();
        }
        public IActionResult Admin()
        {
            var admin = _sessionManager.GetSessionValue("admin");

            if (admin == null)
            {
                return RedirectToAction("AdminLogin");
            }
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }
        public IActionResult AdminAccess()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            if (username == "admin" && password == "1234")
            {
                _sessionManager.SetSessionValue("admin", password);
                return Ok("Access Granted");
            }
            else
            {
                return BadRequest("Access Denied");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
