using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using UA.Integration.SDK;

namespace UA.Integration.SDK
{

    public static class ApplicationHost
    {
        public static IHost AppHost { get; set; }
        public static void AddUAIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {


            //Add Event Hub Service for Consumer and Prodcuer EventHubs
            var blobConnectionString = configuration["InsightCM:BlobConnectionString"];
            var blobContainerName = configuration["InsightCM:BlobContainerName"];
            var consumerConnectionString = configuration["InsightCM:ConsumerConnectionString"];
            var consumerEventHubName = configuration["InsightCM:ConsumerEventHubName"];
            var consumerGroup = configuration["InsightCM:ConsumerGroup"];
            var producerConnectionString = configuration["InsightCM:ProducerConnectionString"];
            var producerEventHubName = configuration["InsightCM:ProducerEventHubName"];


            services.AddSingleton<IEventHubService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventHubService>>();
                return new EventHubService(blobConnectionString, blobContainerName, consumerConnectionString, consumerEventHubName, consumerGroup, producerConnectionString, producerEventHubName, logger);
            });



            //Add Sensor Data Client for Retriving Data
            var storageAccountConnectionString = configuration["InsightCM:ADLSStorageAccountConnectionString"];
            var fileSystemName = configuration["InsightCM:FileSystem"];

            services.AddSingleton<ISensorDataClient>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<SensorDataClient>>();
                return new SensorDataClient(storageAccountConnectionString, fileSystemName, logger);
            });
        }

        public static void ConfigureLogging(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Applicaiton Insights Logging Initialization
            var appInsightsConnectionString = configuration["InsightCM:ApplicationInsightsConnectionString"];
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()  // Make sure to include this
                .WriteTo.Console()
                .WriteTo.ApplicationInsights(appInsightsConnectionString, TelemetryConverter.Traces)
                .CreateLogger();
            services.AddSerilog();

        }
    }
}
