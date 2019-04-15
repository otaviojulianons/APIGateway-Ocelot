using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Resizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseUrls("http://localhost:" + new Random().Next(5050, 6000).ToString())
                .UseStartup<Startup>();
        }
            
    }
}
