using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Resizer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private const string DEFAULT_EXTENSION = ".jpeg";
        private string _fileServerDirectory;

        public ImageController(IConfiguration configuration)
        {
            _fileServerDirectory = configuration.GetValue<string>("FileServerDirectory");
        }

        [HttpPost()]
        public ActionResult<Guid> Post(IFormFile file)
        {
            try
            {
                if (file == null)
                    return BadRequest("File not found.");

                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("File not is a image.");

                if (!Directory.Exists(_fileServerDirectory))
                    Directory.CreateDirectory(_fileServerDirectory);

                var id = Guid.NewGuid();
                var path = GetFullPath(id);

                using (FileStream fileWriteStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    file.OpenReadStream().CopyTo(fileWriteStream);
                }
                return id;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public ActionResult<Guid> Put(IFormFile file, [FromRoute] Guid id)
        {
            try
            {
                if (file == null)
                    return BadRequest("File not found.");

                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("File not is a image.");

                if (!Directory.Exists(_fileServerDirectory))
                    Directory.CreateDirectory(_fileServerDirectory);

                var path = GetFullPath(id);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                using (FileStream fileWriteStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    file.OpenReadStream().CopyTo(fileWriteStream);
                }
                return id;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid file id.");

                var path = GetFullPath(id);

                if (!System.IO.File.Exists(path))
                    return NotFound();

                var bytes = System.IO.File.ReadAllBytes(path);
                
                return new FileContentResult(bytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetFullPath(Guid id) =>
             Path.Combine(_fileServerDirectory, id.ToString() + DEFAULT_EXTENSION);

    }
}