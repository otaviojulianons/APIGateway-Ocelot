using Customers.Models;
using Customers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Customers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private CustomerService _service;

        public CustomerController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet()]
        public IEnumerable<CustomerModel> GetCustomers() => _service.GetCustomers();

        [HttpGet("{id}")]
        public IActionResult GetCustomer([FromRoute] Guid id)
        {
            var customer = _service.GetCustomer(id);

            if (customer == null)
                return NotFound();
            else
                return new JsonResult(customer);
        }
        

        [HttpGet("/Customer/{id}/Avatar")]
        public IActionResult DonloadAvatar([FromRoute] Guid id)
        {
            var bytes = _service.DonloadAvatar(id);
            if (bytes.Length == 0)
                return NotFound();
            return new FileContentResult(bytes, "image/jpeg");
        }


        [HttpPost("/Customer/{id}/Avatar")]
        public IActionResult UploadAvatar([FromRoute] Guid id, IFormFile file)
        {
            try
            {
                if (file == null)
                    throw new Exception("File not found.");

                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("File not is a image.");

                _service.UploadAvatar(id, file.OpenReadStream());
                return Ok("Upload file successful.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}