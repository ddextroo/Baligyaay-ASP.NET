using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Baligyaay.Views.Home
{
    public class Order : PageModel
    {
        private readonly ILogger<Order> _logger;

        public Order(ILogger<Order> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}