using Consul;
using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Resizer.Consumers;
using SharedKernel;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Resizer
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
            //services.Configure<ConsulConfig>(Configuration.GetSection("consulConfig"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Resizer", Version = "v1" });
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Resizer.xml");
                c.IncludeXmlComments(filePath);
            });
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = Configuration["consulConfig:address"];
                consulConfig.Address = new Uri(address);
            }));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMassTransit(x => x.AddConsumer<ResizeConsumer>());

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

                    cfg.ReceiveEndpoint(host, BusConfig.QUEUE_RESIZE, endpoint =>
                    {
                        endpoint.Consumer<ResizeConsumer>(provider);
                    });
                });
            });
            services.AddHostedService<BusServiceHosted>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
            app.UseMvc();

            //app.RegisterWithConsul(lifetime);
        }

      
    }
}
