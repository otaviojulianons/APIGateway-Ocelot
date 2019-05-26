using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SharedKernel
{
    public static class ConsulExtensions
    {
        public static void RegisterWithConsul(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            // Setup logger
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            try
            {

                var consulClient = app.ApplicationServices
                                    .GetRequiredService<IConsulClient>();

                var consulConfig = app.ApplicationServices
                                    .GetRequiredService<IOptions<ConsulConfig>>();

                // Get server IP address
                var features = app.Properties["server.Features"] as FeatureCollection;
                var addresses = features.Get<IServerAddressesFeature>();
                string ip = GetLocalIPAddress();
                var address = addresses.Addresses.FirstOrDefault()?.Replace("+",ip);

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = $"{address}/status",
                    Timeout = TimeSpan.FromSeconds(5)
                };
                logger.Log(LogLevel.Warning, $"Addess check: {httpCheck.HTTP}");

                // Register service with consul
                var uri = new Uri(address);
                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    ID = $"{consulConfig.Value.ServiceName}-{Guid.NewGuid().ToString()}",
                    Name = consulConfig.Value.ServiceName,
                    Address = ip,
                    Port = uri.Port
                };

                logger.LogInformation($"Registering with Consul service: {registration.Name}, port: {registration.Port}");

                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();

                lifetime.ApplicationStopping.Register(() => {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registering with Consul error.");
            }

        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
