using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
namespace Baligyaay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("images")]
        public IActionResult GetImages()
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                return NotFound("Uploads directory does not exist.");
            }

            var imageFiles = Directory.GetFiles(uploadsPath)
                .Select(file => new ImageListModel
                {
                    Url = $"{Request.Scheme}://{Request.Host}/uploads/{Path.GetFileName(file)}"
                })
                .ToList();

            return Ok(imageFiles);
        }
    }
}