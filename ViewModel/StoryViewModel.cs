using Newtonsoft.Json;

namespace WorldDiabetesFoundation.Core.ViewModel;

public class StoryViewModel
{
    [JsonProperty("link")] 
    public string link { get; set; }
    [JsonProperty("id")] 
    public int id { get; set; }
    [JsonProperty("key")] 
    public string __Key { get; set; }
    [JsonProperty("image")] 
    public ImageData image { get; set; }
    [JsonProperty("tagPicker")] 
    public string tagPicker { get; set; }
    [JsonProperty("date")] 
    public string date { get; set; }
    [JsonProperty("author")] 
    public string author { get; set; }
    [JsonProperty("title")] 
    public string title { get; set; }
    [JsonProperty("description")] 
    public string description { get; set; }
}