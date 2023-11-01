using Newtonsoft.Json;

namespace WorldDiabetesFoundation.Core.ViewModel;

public class ProjectViewModel
{
    public string parent { get; set; }
    [JsonProperty("id")]
    public int id { get; set; }
    
    [JsonProperty("key")] 
    public string __Key { get; set; }
    
    [JsonProperty("link")] 
    public string link { get; set; }
    
    [JsonProperty("image")] 
    public ImageData image { get; set; }
    
    [JsonProperty("project_id")]
    public string project_id { get; set; }
    
    [JsonProperty("title")]
    public string title { get; set; }
    
    [JsonProperty("description")]
    public string description { get; set; }
    
    [JsonProperty("objectives")]
    public string objectives { get; set; }

    [JsonProperty("approach")]
    public string approach { get; set; }

    [JsonProperty("expected_results")]
    public string expected_results { get; set; }
    
    [JsonProperty("countries")]
    public List<string> formattedCountries { get; set; }
    
    [JsonProperty("regions")]
    public List<string> formattedRegions{ get; set; }
    
    [JsonProperty("locations")]
    public List<GeoCoordinate> formattedLocations { get; set; }
    
    [JsonProperty("intervention_areas")]
    public List<string> formattedInterventionAreas { get; set; }
    
    [JsonProperty("status")]
    public string status { get; set; }
    
    [JsonProperty("period")]
    public string period { get; set; }
    
    [JsonProperty("unsorted_locations")]
    public string locations { get; set; }
    
    [JsonProperty("unsorted_countries")]
    public string countries { get; set; }
    
    [JsonProperty("unsorted_intervention_areas")]
    public string intervention_areas { get; set; }
    
    [JsonProperty("unsorted_regions")]
    public string regions { get; set; }
}

public class GeoCoordinate
{
    public float latitude { get; set; }
    public float longitude { get; set; }
}

public class Crop
{
    public string alias { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public Coordinates coordinates { get; set; }
}

public class Coordinates
{
    public double? x1 { get; set; }
    public double? y1 { get; set; }
    public double? x2 { get; set; }
    public double? y2 { get; set; }
}

public class ImageData
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string extension { get; set; }
    public string url { get; set; }
    public object focalPoint { get; set; }
    public List<Crop> crops { get; set; }
}
