using Newtonsoft.Json;

namespace WorldDiabetesFoundation.Core.ViewModel;

public class DefaultGeneralSearchView
{
    [JsonProperty("id")]
    public int id { get; set; }
    [JsonProperty("link")] 
    public string link { get; set; }
    [JsonProperty("title")] 
    public string title { get; set; }
    [JsonProperty("description")] 
    public string description { get; set; }
    [JsonProperty("contentGrid")] 
    public string contentGrid { get; set; }
}