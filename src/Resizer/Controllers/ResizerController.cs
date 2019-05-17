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
        public IActionResult Post(IFormFile file, [FromRoute] float percent = 50)
        {
            if (file == null)
                return NotFound();

            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var newImage = ImageUtils.Resize(memoryStream.ToArray(), percent);
            return new FileContentResult(newImage, "image/jpeg");
        }
    }
}