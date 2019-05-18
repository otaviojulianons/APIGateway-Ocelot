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
        [HttpPost("{percent}")]
        public IActionResult Post(IFormFile file, [FromRoute] float percent)
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
    }
}