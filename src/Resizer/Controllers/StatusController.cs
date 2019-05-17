using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Resizer.Controllers
{
    [Route("[controller]")]
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
                ApiName = "resizer.api",
                Ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                Host = Request.Host.Host,
                Port = Request.Host.Port,

            };
            return status;
        }

    }
}
