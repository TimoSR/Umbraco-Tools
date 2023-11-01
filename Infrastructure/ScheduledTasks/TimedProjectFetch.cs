using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using WorldDiabetesFoundation.Core.DataTransferObjects;
using WorldDiabetesFoundation.Core.Infrastructure.Integration;
using WorldDiabetesFoundation.Core.Infrastructure.Integration.Interfaces;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Importers;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities;
using WorldDiabetesFoundation.Web.Infrastructure;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Enum;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Core.Infrastructure.ScheduledTasks;

public class TimedProjectFetch : IHostedService, IDisposable
{
    private int _executionCount = 0;
    private readonly ILogger<TimedProjectFetch> _logger;
    private Timer _timer;
    private readonly string _requestUrl;
    private readonly IHttpClient _apiDataFetcher;
    private readonly IRequestBody _requestBody;
    private readonly ProjectDataImporter _projectDataImporter;
    private readonly string _secretKey;
    private readonly JsonSerializer _serializer;
    
    public TimedProjectFetch(
        ILogger<TimedProjectFetch> logger, 
        ApiDataFetcher apiDataFetcher, 
        IProjectSettings projectSettings, 
        ProjectDataImporter projectDataImporter,
        JsonSerializer serializer)
    {
        _logger = logger;
        _apiDataFetcher = apiDataFetcher;
        _requestUrl = projectSettings.ApiUrl;
        _requestBody = projectSettings.RequestBody;
        _secretKey = projectSettings.SecretKey;
        _projectDataImporter = projectDataImporter;
        _serializer = serializer;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        FetchWdkApi(null);

        string timeZoneId;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            timeZoneId = "Central European Standard Time";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            timeZoneId = "Europe/Copenhagen";
        }
        else
        {
            throw new Exception("Unsupported platform");
        }

        // // Danish timezone
        var danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        // Calculate the time until 1AM in Danish timezone
        var nowInDanishTime = TimeZoneInfo.ConvertTime(DateTime.Now, danishTimeZone);
        var next1AmInDanishTime = nowInDanishTime.Date.Add(new TimeSpan(25, 0, 0)); // Get 1AM of the next day
        var initialInterval = next1AmInDanishTime - nowInDanishTime;

        // Start the timer so it triggers at the next 1AM Danish time and every 24 hours thereafter
        _timer = new Timer(FetchWdkApi, null, initialInterval, TimeSpan.FromHours(24));

        // example, to run the service every 15 seconds:
        // _timer = new Timer(FetchWdkApi, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

        return Task.CompletedTask;
    }

    private async void FetchWdkApi(object state)
    {
        try
        {
            var count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation("Timed Hosted Service is working. Fetch Count: {Count}", count);

            var result= await _apiDataFetcher.CallWebApiWithHeader(_requestUrl, _secretKey);

            var resultValue = result.Value;
        
            _logger.LogInformation(resultValue);

            var projects = _serializer.DeserializeModelList<ProjectDTO>(resultValue);
        
            if (result.Status == ResultStatus.Success)
            {
                _projectDataImporter.ProcessAndStoreProjects(projects);
            }
            else
            {
                Console.WriteLine($"Error: {result.Error.Message}");
        
                if (result.Error.StatusCode.HasValue)
                {
                    _logger.LogInformation($"{result.Error.StatusCode.Value}");
                }
            }
        }
        catch (Exception ex) // Added specific exception type
        {
            _logger.LogError(ex, "An error occurred while fetching data from the API.");
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}