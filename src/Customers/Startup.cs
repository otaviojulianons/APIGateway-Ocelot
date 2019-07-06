using Consul;
using Customers.Consumers;
using Customers.Services;
using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;
using SharedKernel;
using SharedKernel.Log;
using SharedKernel.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Customers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Configuration.ConfigureSerilog();
            services.Configure<ConsulConfig>(Configuration.GetSection("consulConfig"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Customer", Version = "v1" });
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Customers.xml");
                c.IncludeXmlComments(filePath);
            });
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = Configuration["consulConfig:address"];
                consulConfig.Address = new Uri(address);
            }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMassTransit(x => x.AddConsumer<ResizeResultConsumer>());

            BusConfig busConfig = new BusConfig(
                 Configuration["rabbitmq:username"],
                 Configuration["rabbitmq:password"],
                 Configuration["rabbitmq:host"]
            );
            services.AddSingleton(provider =>
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(busConfig.Host, h =>
                    {
                        h.Username(busConfig.Username);
                        h.Password(busConfig.Password);
                    });
                    cfg.ReceiveEndpoint(host, BusConfig.QUEUE_RESIZE_RESULT, endpoint =>
                    {
                        endpoint.Consumer<ResizeResultConsumer>(provider);
                    });
                });
            });
            services.AddHostedService<BusServiceHosted>();
            services.AddSingleton<CustomerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //app.UseMiddleware<LogRequestMiddleare>();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
            app.UseMvc();
            app.RegisterWithConsul(lifetime);
        }
    }
}
