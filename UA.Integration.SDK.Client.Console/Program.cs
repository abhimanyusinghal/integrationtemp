// See https://aka.ms/new-console-template for more information
Console.WriteLine("Example application on how to use the SDK");

//Starting EventHubService
var eventHubService = EventHubServiceFactory.Create(
    "your_blob_storage_connection_string",
    "your_blob_container_name",
    "your_event_hubs_connection_string",
    "your_event_hub_name",
    "your_consumer_group");

//Creating the SensorDataClient
var apiBaseUrl = "https://example.com/api&code"; // This should be replaced with the actual URL
var sensorDataClient = SensorDataClientFactory.Create(apiBaseUrl);


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
    var sasUrl = sensorDataClient.GenerateSingleSasUrl(e.SensorSerialNumber, e.Timestamp, e.MeasurementType).Result;
    Console.WriteLine($"SAS URL: {sasUrl}");

};

eventHubService.Start();


Console.WriteLine("Press any key to stop the Consumer");
Console.ReadLine();
eventHubService.Stop();


Console.WriteLine("Fetching sensor messages");
var sensorMessages = await sensorDataClient.FetchSensorMessagesForDateRange("sensorSerialNumber", 1640995200, 1641081600);

foreach (var sensorMessage in sensorMessages)
{
    Console.WriteLine($"SensorId: {sensorMessage.SensorId}");
    Console.WriteLine($"Timestamp: {sensorMessage.Timestamp}");
    Console.WriteLine($"TraceParentId: {sensorMessage.TraceParentId}");
    Console.WriteLine($"TraceId: {sensorMessage.TraceId}");
    Console.WriteLine($"SpanId: {sensorMessage.SpanId}");
    Console.WriteLine($"SensorType: {sensorMessage.SensorType}");
    Console.WriteLine($"Scope: {sensorMessage.Scope}");
    Console.WriteLine($"GatewayId: {sensorMessage.GatewayId}");
}

Console.WriteLine("Press any key to stop the application");
Console.ReadLine();