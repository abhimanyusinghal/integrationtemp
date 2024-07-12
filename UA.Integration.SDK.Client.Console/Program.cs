using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using UA.Integration.SDK;


ApplicationHost.AppHost = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                var settings = config.Build();
                var endpoint = settings["AzureAppConfigurationEndpoint"];
                var managedIdentityClientId = settings["ManagedIdentityClientId"];

                config
                .AddAzureAppConfiguration(options =>
                {

#if LOCAL
  options
                        .Connect(new Uri(endpoint), new DefaultAzureCredential())
                        .ConfigureKeyVault(keyVaultConfig =>
                        {
                            keyVaultConfig.SetCredential(new DefaultAzureCredential());
                        });
#else
                    //Settings for Running in Cloud VM Assigned for Insight CM
                    options
                        .Connect(new Uri(endpoint), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId }))
                        .ConfigureKeyVault(keyVaultConfig =>
                        {
                            keyVaultConfig.SetCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId }));
                        });
#endif
                });
            })
             .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureLogging(hostContext.Configuration);
                    services.AddUAIntegrationServices(hostContext.Configuration);
                })
                .Build();

Console.WriteLine("Example application on how to use the SDK");

//Starting EventHubService
var eventHubService = ApplicationHost.AppHost.Services.GetRequiredService<IEventHubService>();


var sensorDataClient = ApplicationHost.AppHost.Services.GetRequiredService<ISensorDataClient>();


//Subscribe to the RawDataAvailableEvent
eventHubService.NewMessage += (sender, e) =>
{
    Console.WriteLine($"New message received: {e.SensorSerialNumber}");
    Console.WriteLine($"Timestamp: {e.Timestamp}");
    Console.WriteLine($"MeasurementType: {e.MeasurementType}");
    Console.WriteLine($"SensorType: {e.SensorType}");
    Console.WriteLine($"GatewaySerialNumber: {e.GatewaySerialNumber}");
    Console.WriteLine($"SensorScope: {e.SensorScope}");


    //getting a single SAS URL
    var sasUrl = sensorDataClient.GenerateSingleSasUrl(e.SensorSerialNumber, e.MeasurementType, e.Timestamp).Result;
    Console.WriteLine($"SAS URL: {sasUrl}");

    //gettin multiple SAS Urls for Multiple timestamps
    var sasUrls1 = sensorDataClient.GenerateMultipleSasUrls(e.SensorSerialNumber, e.MeasurementType, new List<long>() { e.Timestamp, e.Timestamp, e.Timestamp }).Result;
    Console.WriteLine($"SAS URL: {sasUrls1}");

    //getting multiple SAS Urls for a range
    var sasUrls2 = sensorDataClient.GenerateSasUrlsForDateRange(e.SensorSerialNumber, e.MeasurementType, e.Timestamp - 1000, e.Timestamp + 100).Result;
    Console.WriteLine($"SAS URL: {sasUrls2}");


    //Posting Extracted Featues Bcak to UA
    eventHubService.SendFeatureValueMessageAsync(new FeatureValueMessage()
    {
        Axial = new List<FeatureValue>(),
        Horz = new List<FeatureValue>(),
        SerialNumber = e.SensorSerialNumber,
        Timestamp = e.Timestamp,
        Vert = new List<FeatureValue>()
    });

};

eventHubService.Start();


Console.WriteLine("Press any key to stop the Consumer");
Console.ReadLine();
eventHubService.Stop();


Console.WriteLine("Press any key to stop the application");
Console.ReadLine();