

    using Azure.Storage;
    using Azure.Storage.Files.DataLake;
    using Azure.Storage.Sas;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Threading.Tasks;

namespace UA.Integration.SDK
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Files.DataLake;
    using Azure.Storage.Sas;

    using Microsoft.Extensions.Logging;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Configuration;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class SensorDataClient : ISensorDataClient
    {
        private readonly DataLakeServiceClient _dataLakeServiceClient;
        private readonly ILogger<SensorDataClient> _logger;
        private readonly string _fileSystemName;

        private readonly IMongoCollection<BsonDocument> _collection;


        public SensorDataClient(string storageAccountConnectionString, string fileSystemName, ILogger<SensorDataClient> logger,string mongoConnectionString, string mongoDbName,string mongoCollectionName)
        {
            _dataLakeServiceClient = new DataLakeServiceClient(storageAccountConnectionString);
            _fileSystemName = fileSystemName;
            _logger = logger;

            var client = new MongoClient(mongoConnectionString);
            var database = client.GetDatabase(mongoDbName);
            _collection = database.GetCollection<BsonDocument>(mongoCollectionName);
        }

        public Task<List<SensorFeatureData>> FetchMultipleSensorMessages(string sensorSerialNumber, List<long> timestamps)
        {
            throw new NotImplementedException();
        }

        public Task<List<SensorFeatureData>> FetchSensorMessagesForDateRange(string sensorSerialNumber, long startDate, long endDate)
        {
            throw new NotImplementedException();
        }

        public async Task<SensorFeatureData> FetchSingleSensorMessage(string sensorSerialNumber, long unixEpochTimestamp)
        {

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("sensorId", sensorSerialNumber),
                Builders<BsonDocument>.Filter.Eq("timestamp", unixEpochTimestamp)
            );

            var document = await _collection.Find(filter).FirstOrDefaultAsync();

            if (document == null)
                return null;

            // Assuming you have a method to convert a BsonDocument to your SensorFeatureData object
            return DeserializeSensorFeatureData(document);
        }

        private SensorFeatureData DeserializeSensorFeatureData(BsonDocument document)
        {
            var data = new SensorFeatureData
            {
                Id = document["_id"].ToString(),
                SensorId = document.TryGet("sensorId", out string sensorId) ? sensorId : default,
                Timestamp = document.TryGet("timestamp", out long timestamp) ? timestamp : default,
                TraceParentId = document.TryGet("traceParentId", out string traceParentId) ? traceParentId : default,
                TraceId = document.TryGet("traceId", out string traceId) ? traceId : default,
                SpanId = document.TryGet("spanId", out string spanId) ? spanId : default,
                SensorType = document.TryGet("sensorType", out string sensorType) ? sensorType : default,
                Scope = document.TryGet("scope", out string scope) ? scope : default,
                GatewayId = document.TryGet("gatewayId", out string gatewayId) ? gatewayId : default,
                GwTime = document.TryGet("gwTime", out long gwTime) ? gwTime : default,
                Tags = document.Contains("tags") ? document["tags"].AsBsonArray.Select(t => t.AsString).ToList() : new List<string>(),
                Config = document.Contains("config") ? DeserializeDictionary(document["config"].AsBsonDocument) : new Dictionary<string, object>(),
                CreatedDate = document.TryGet("createdDate", out long createdDate) ? createdDate : default,
                CoRelationId = document.TryGet("co-relationId", out string coRelationId) ? coRelationId : default,
                CloudTimestamp = document.TryGet("cloudTimestamp", out long cloudTimestamp) ? cloudTimestamp : default,
                TenantId = document.TryGet("tenantId", out string tenantId) ? tenantId : default,
                ObjectClass = document.TryGet("objectClass", out string objectClass) ? objectClass : default,
                ObjectType = document.TryGet("objectType", out string objectType) ? objectType : default,
                ObjectSubType = document.TryGet("objectSubType", out string objectSubType) ? objectSubType : default,
                PreviousHealthStatus = document.TryGet("previousHealthStatus", out string previousHealthStatus) ? previousHealthStatus : default,
                SchemaVersion = document.TryGet("schemaVersion", out string schemaVersion) ? schemaVersion : default,
                FirmwareVer = document.TryGet("firmwareVer", out string firmwareVer) ? firmwareVer : default,
                SensorParameters = document.Contains("sensorParameters") ? DeserializeDictionary(document["sensorParameters"].AsBsonDocument) : new Dictionary<string, object>()
            };

            return data;
        }

        private Dictionary<string, object> DeserializeDictionary(BsonDocument parameters)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var parameter in parameters)
            {
                dictionary[parameter.Name] = BsonTypeMapper.MapToDotNetValue(parameter.Value);
            }
            return dictionary;
        }


        public async Task<List<string>> GenerateMultipleSasUrls(string sensorSerialNumber, MeasurementType measurementType, List<long> timestamps)
        {
            var sasUrls = new List<string>();
            var notFoundTimestamps = new List<long>();

            foreach (var timestamp in timestamps)
            {
                try
                {
                    var sasUrl = await GenerateSingleSasUrl(sensorSerialNumber, measurementType, timestamp);
                    sasUrls.Add(sasUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate SAS URL for timestamp {Timestamp}", timestamp);
                    notFoundTimestamps.Add(timestamp);
                }
            }

            if (notFoundTimestamps.Any())
            {
                var notFoundTimestampsString = string.Join(", ", notFoundTimestamps);
                _logger.LogError("Failed to generate SAS URLs for the following timestamps: {NotFoundTimestamps}", notFoundTimestampsString);
                throw new Exception($"Failed to generate SAS URLs for the following timestamps: {notFoundTimestampsString}");
            }

            return sasUrls;
        }

        public async Task<List<string>> GenerateSasUrlsForDateRange(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate)
        {
            var sasUrls = new List<string>();
            var notFoundFiles = new List<string>();

            try
            {
                var startDateTime = DateTimeOffset.FromUnixTimeSeconds(startDate).UtcDateTime;
                var endDateTime = DateTimeOffset.FromUnixTimeSeconds(endDate).UtcDateTime;

                _logger.LogInformation("Generating SAS URLs for date range {StartDate} to {EndDate}", startDateTime, endDateTime);

                var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(_fileSystemName);

                for (var year = startDateTime.Year; year <= endDateTime.Year; year++)
                {
                    var startMonth = (year == startDateTime.Year) ? startDateTime.Month : 1;
                    var endMonth = (year == endDateTime.Year) ? endDateTime.Month : 12;

                    for (var month = startMonth; month <= endMonth; month++)
                    {
                        var startDay = (year == startDateTime.Year && month == startDateTime.Month) ? startDateTime.Day : 1;
                        var endDay = (year == endDateTime.Year && month == endDateTime.Month) ? endDateTime.Day : DateTime.DaysInMonth(year, month);

                        for (var day = startDay; day <= endDay; day++)
                        {
                            var startHour = (year == startDateTime.Year && month == startDateTime.Month && day == startDateTime.Day) ? startDateTime.Hour : 0;
                            var endHour = (year == endDateTime.Year && month == endDateTime.Month && day == endDateTime.Day) ? endDateTime.Hour : 23;

                            for (var hour = startHour; hour <= endHour; hour++)
                            {
                                var startMinute = (year == startDateTime.Year && month == startDateTime.Month && day == startDateTime.Day && hour == startDateTime.Hour) ? startDateTime.Minute : 0;
                                var endMinute = (year == endDateTime.Year && month == endDateTime.Month && day == endDateTime.Day && hour == endDateTime.Hour) ? endDateTime.Minute : 59;

                                for (var minute = startMinute; minute <= endMinute; minute++)
                                {
                                    var directoryPath = $"{sensorSerialNumber}/{measurementType}/{year:D4}/{month:D2}/{day:D2}/{hour:D2}";
                                    var prefix = $"{minute:D2}_";

                                    await foreach (var pathItem in fileSystemClient.GetPathsAsync(directoryPath, false))
                                    {
                                        if (!pathItem.IsDirectory.HasValue || !pathItem.IsDirectory.Value)
                                        {
                                            var fileName = System.IO.Path.GetFileName(pathItem.Name);
                                            if (fileName.StartsWith(prefix))
                                            {
                                                try
                                                {
                                                    var parts = fileName.Split('_');
                                                    if (parts.Length >= 3 && long.TryParse(parts[2].Replace(".json", ""), out long fileTimestamp))
                                                    {
                                                        if (fileTimestamp >= startDate && fileTimestamp <= endDate)
                                                        {
                                                            var sasUrl = await GenerateSingleSasUrl(sensorSerialNumber, measurementType, fileTimestamp);
                                                            sasUrls.Add(sasUrl);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.LogError(ex, "Failed to generate SAS URL for file {FileName}", pathItem.Name);
                                                    notFoundFiles.Add(pathItem.Name);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (notFoundFiles.Any())
                {
                    var notFoundFilesString = string.Join(", ", notFoundFiles);
                    _logger.LogError("Failed to generate SAS URLs for the following files: {NotFoundFiles}", notFoundFilesString);
                    throw new Exception($"Failed to generate SAS URLs for the following files: {notFoundFilesString}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate SAS URLs for date range {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }

            return sasUrls;
        }


        public async Task<string> GenerateSingleSasUrl(string sensorSerialNumber, MeasurementType measurementType, long unixEpochTimestamp)
        {
            try
            {
                // Convert the timestamp to DateTime
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixEpochTimestamp);
                DateTime dateTime = dateTimeOffset.UtcDateTime;

                // Extract the minute and second from the timestamp
                int minute = dateTime.Minute;
                int second = dateTime.Second;

                // Construct the path based on the given parameters
                string path = $"{sensorSerialNumber}/{measurementType}/{dateTime:yyyy}/{dateTime:MM}/{dateTime:dd}/{dateTime:HH}/{minute:D2}_{second:D2}_{unixEpochTimestamp}.json";

                var fileSystemClient = _dataLakeServiceClient.GetFileSystemClient(_fileSystemName);
                var fileClient = fileSystemClient.GetFileClient(path);

                if (!await fileClient.ExistsAsync())
                {
                    _logger.LogWarning("File {FileName} does not exist.", path);
                    throw new Exception($"File {path} does not exist.");
                }

                var sasBuilder = new DataLakeSasBuilder
                {
                    FileSystemName = fileSystemClient.Name,
                    Path = fileClient.Path,
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                sasBuilder.SetPermissions(DataLakeSasPermissions.Read);

                var sasUri = fileClient.GenerateSasUri(sasBuilder);

                _logger.LogInformation("Generated SAS URL for file {FileName}.", path);
                return sasUri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate SAS URL.");
                throw;
            }
        }

     
    }
}
