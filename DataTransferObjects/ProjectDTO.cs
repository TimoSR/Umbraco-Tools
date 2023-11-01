using Newtonsoft.Json;

namespace WorldDiabetesFoundation.Core.DataTransferObjects;

public class ProjectDTO
{
    [JsonProperty("Project Title")]
    public string title { get; set; }

    [JsonProperty("Project nr")]
    public string project_id { get; set; }
    
    [JsonProperty("Objectives")]
    public string objectives { get; set; }

    [JsonProperty("Approach")]
    public string approach { get; set; }

    [JsonProperty("Expected results")]
    public string expected_results { get; set; }

    [JsonProperty("Project Status")]
    public string status { get; set; }

    [JsonProperty("InterventionAreas")]
    public string intervention_areas { get; set; }
    
    [JsonProperty("Region")]
    public string regions { get; set; }

    [JsonProperty("Country")]
    public string countries { get; set; }

    [JsonProperty("Project period")]
    public string period { get; set; }

    [JsonProperty("Partners")]
    public string partners { get; set; }

    [JsonProperty("Project Budget")]
    public string budget { get; set; }

    [JsonProperty("WDF Contribution")]
    public string wdf_contribution { get; set; }

    [JsonProperty("Notes")]
    public string notes { get; set; }

    [JsonProperty("Results upon completion")]
    public string results_upon_completion { get; set; }

    [JsonProperty("Cities")]
    public IEnumerable<City> cities { get; set; }
    
    public string locations
    {
        get
        {
            return string.Join("; ", cities.Select(c => $"{c.Lat}, {c.Lng}"));
        }
    }
}

public class City
{
    public string city { get; set; }
    public string Lat { get; set; }
    public string Lng { get; set; }
}




