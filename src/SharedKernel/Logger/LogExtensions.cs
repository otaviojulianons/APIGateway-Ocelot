using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;


namespace SharedKernel.Logger
{
    public static class LogExtensions
    {
        public static void ConfigureSerilog(this IConfiguration configuration)
        {      
            var configuracaoLog = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext();

            if(configuration.GetValue<bool>("serilog:console:enabled")) 
            {
                configuracaoLog.WriteTo.Console(new ElasticsearchJsonFormatter());
            }
            if(configuration.GetValue<bool>("serilog:file:enabled")) 
            {
                var path = configuration.GetValue<string>("serilog:file:path");;
                configuracaoLog.WriteTo.File(
                    new ElasticsearchJsonFormatter(),
                    path,
                    rollingInterval: RollingInterval.Day
                );
            }            
            if(configuration.GetValue<bool>("serilog:mongodb:enabled")) 
            {
                var mongoUri = configuration.GetValue<string>("serilog:mongodb:url");
                configuracaoLog.WriteTo.MongoDB(mongoUri);
            }
            if(configuration.GetValue<bool>("serilog:elastic:enabled")) 
            {
                var elasticUri = configuration.GetValue<string>("serilog:elastic:url");
                configuracaoLog.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                    TemplateName = "serilog",
                    IndexFormat = "serilog-{0:yyyy.MM}"
                });
            }
            Serilog.Log.Logger = configuracaoLog.CreateLogger();
        }        
    }
}