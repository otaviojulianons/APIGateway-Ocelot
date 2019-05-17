using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("/")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private IHttpContextAccessor _accessor;

        public StatusController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        [HttpGet]
        public ActionResult<object> Get()
        {
            var status = new
            {
                ApiName = "gateway",
                Ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                Host = Request.Host.Host,
                Port = Request.Host.Port,

            };
            return status;
        }
    }
}
