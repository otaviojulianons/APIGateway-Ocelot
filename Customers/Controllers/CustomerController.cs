using Customers.Models;
using Customers.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Messaging.Messages;
using System;
using System.IO;

namespace Customers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private const string IMAGES_PATH = "Images";
        private IBusControl _bus;
        private CustomerService _service;

        public CustomerController(IBusControl bus, CustomerService service)
        {
            _bus = bus;
            _service = service;
        }

        [HttpPost]
        public UploadResponseModel UploadSelfie(IFormFile file)
        {
            try
            {
                var id = Guid.NewGuid();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), IMAGES_PATH);
               
                if (!Directory.Exists(IMAGES_PATH))
                    Directory.CreateDirectory(IMAGES_PATH);

                var extension = Path.GetExtension(file.FileName);
                filePath = Path.Combine(filePath, id.ToString() + extension);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    file.CopyTo(stream);

                var imageInfo = new UploadResponseModel()
                {
                    SelfieId = id,
                    Message = "Upload file successful.",
                    Size = Math.Round((decimal)file.Length / (1024 * 1024), 2)
                };

                _service.Images.Add(id, imageInfo);

                _bus.Send(new ImageResizeMessage() { Id = id, FilePath = filePath });

                return imageInfo;
            }
            catch (Exception ex)
            {
                return new UploadResponseModel() { Message = ex.Message };
            }
        }

        [HttpGet("{filename}")]
        public IActionResult DownloadSelfie([FromRoute] string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), IMAGES_PATH, filename);
            if (System.IO.File.Exists(filePath))
                return new FileContentResult(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
            else
                return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult SelfieInfo([FromRoute] Guid id)
        {
            if (_service.Images.ContainsKey(id))
                return new JsonResult(_service.Images[id]);
            else
                return NotFound();
        }
    }
}