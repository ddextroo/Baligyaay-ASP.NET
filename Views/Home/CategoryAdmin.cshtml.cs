using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Baligyaay.Views.Home
{
    public class CategoryAdmin : PageModel
    {
        private readonly ILogger<CategoryAdmin> _logger;

        public CategoryAdmin(ILogger<CategoryAdmin> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}