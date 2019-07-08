using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SharedKernel.Logger
{
    public class LogRequestMiddleare
    {
        private RequestDelegate _next;

        public LogRequestMiddleare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = new 
            {
                ip = context.Connection.RemoteIpAddress.ToString(),
                method = context.Request.Method,
                path = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                queryString = context.Request.QueryString.ToString(),
                headers = context.Request.Headers.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString())
            };

            var response = new
            {
                statusCode = context.Response.StatusCode,
                server = Environment.MachineName
            };

            var logRequest = new 
            {
                request,
                response,
                dateTime = DateTime.Now
            };

            Serilog.Log.Information("Lod de requisição: {@logRequest}", logRequest);
            await _next(context);
        }

    }
}