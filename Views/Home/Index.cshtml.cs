using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Baligyaay.Views.Home
{
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        public string Email { get; set; }

        public Index(ILogger<Index> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Email = HttpContext.Session.GetString("Email")!;
        }
    }
}