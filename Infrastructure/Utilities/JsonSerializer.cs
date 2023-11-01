using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WorldDiabetesFoundation.Core.Infrastructure.Utilities;

public class JsonSerializer
{
    private readonly ILogger _logger;

    public JsonSerializer(ILogger<JsonSerializer> logger)
    {
        _logger = logger;
    }

    public IEnumerable<T>? DeserializeModelList<T>(string responseBody)
    {
        try
        {
            _logger.LogInformation("Attempting to deserialize JSON string");
            return JsonConvert.DeserializeObject<IEnumerable<T>>(responseBody);
        }
        catch (JsonReaderException ex)
        {
            _logger.LogError("An error occurred during deserialization: {Exception}", ex);
            throw;
        }
    }
    
    public string SerializeToIndentedJson<T>(T responseBody)
    {
        return JsonConvert.SerializeObject(responseBody, Formatting.Indented);
    }
}