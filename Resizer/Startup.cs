﻿using Consul;
using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resizer.Consumers;
using SharedKernel;
using System;

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
            services.Configure<ConsulConfig>(Configuration.GetSection("consulConfig"));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = Configuration["consulConfig:address"];
                consulConfig.Address = new Uri(address);
            }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMassTransit(x => x.AddConsumer<ResizeConsumer>());

            BusConfig busConfig = new BusConfig("guest", "guest", "rabbitmq://localhost:32769/");

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
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.RegisterWithConsul(lifetime);
        }

      
    }
}
