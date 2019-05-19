using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resizer.Utils;
using System.IO;

namespace Resizer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResizerController : ControllerBase
    {
        [HttpPost("/Resizer/{percent}%")]
        public IActionResult ResizeByPercent(IFormFile file, [FromRoute] float percent)
        {
            if (file == null)
                return BadRequest("Invalid file.");

            if (percent <= 0 || percent >= 100)
                return BadRequest("Invalid percent.");

            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var newImage = ImageUtils.Resize(memoryStream.ToArray(), percent);
            return new FileContentResult(newImage, "image/jpeg");
        }

        [HttpPost("/Resizer/{height}x{width}")]
        public IActionResult ResizeByFixedSize(IFormFile file, [FromRoute] int height, [FromRoute] int width)
        {
            if (file == null)
                return BadRequest("Invalid file.");

            if (height <= 0 || width <= 0)
                return BadRequest("Invalid size.");

            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var newImage = ImageUtils.Resize(memoryStream.ToArray(), height, width);
            return new FileContentResult(newImage, "image/jpeg");
        }
    }
}