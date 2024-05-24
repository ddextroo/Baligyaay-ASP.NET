using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Baligyaay.Views.Home
{
    public class CustomerAdmin : PageModel
    {
        private readonly ILogger<CustomerAdmin> _logger;

        public CustomerAdmin(ILogger<CustomerAdmin> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}