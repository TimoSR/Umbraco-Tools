using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities;
using WorldDiabetesFoundation.Core.ViewModel;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;

public class ProjectSearcher
{
    private readonly ExamineSearcher _examineSearcher;
    private readonly DataExtractor _dataExtractor;

    public ProjectSearcher(
        ExamineSearcher examineSearcher,
        DataExtractor dataExtractor)
    {
        _examineSearcher = examineSearcher;
        _dataExtractor = dataExtractor;
    }

    public string SearchProjects(Dictionary<string, string> filters, string generalSearch)
    {
        var fieldsToSearch = new[] { "title","project_id","countries","description","objectives", "approach","expected_results"};

        var searchResult = _examineSearcher.SearchByMultipleFields<ProjectViewModel>(ProjectPage.ModelTypeAlias, fieldsToSearch, filters, generalSearch);

        var transformedResult = TransformProjectResults(searchResult);
        
        var serializedObject = SerializeToIndentedJson(transformedResult);
        
        var projectJsonArray = JArray.Parse(serializedObject);

        return projectJsonArray.ToString();
    }

    private List<ProjectViewModel> TransformProjectResults(string serializedResults)
    {
        var projectList = JsonConvert.DeserializeObject<List<ProjectViewModel>>(serializedResults);
        if (projectList == null)
        {
            throw new JsonSerializationException("Failed to deserialize the project list.");
        }

        try 
        {
            foreach (var project in projectList)
            {
                TransformLocations(project);
                TransformCountries(project);
                TransformInterventionAreas(project);
                TransformRegions(project);

                var image =_dataExtractor.GetImageWithCropsById(project.id);

                if (image != null)
                {
                    // Deserialize the image string into an IndentedImageData object
                    var deserializedImage = JsonConvert.DeserializeObject<ImageData>(image);
                    // Replace the original image string with the indented JSON string
                    project.image = deserializedImage;
                }

                project.link = _dataExtractor.GetUrlFromNodeId(project.id);
                project.parent = _dataExtractor.FindParentNode(project.id).ContentType.Alias;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return projectList;
    }

    public void TransformLocations(ProjectViewModel project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project), "testProject cannot be null");
        }
    
        project.formattedLocations = new List<GeoCoordinate>();
    
        // Check if testProject.locations is null
        if (string.IsNullOrEmpty(project.locations))
        {
            return; // or log a warning, or throw an exception
        }

        var coordinatePairs = project.locations.Split(";");

        foreach (var coordinatePair in coordinatePairs)
        {

            string[] coordinates;
            string latitude;
            string longitude;

            coordinates = coordinatePair.Split(",");

            if (coordinates.Length > 2)
            {
                latitude = $"{coordinates[0]}.{coordinates[1]}";
                longitude = $"{coordinates[2]}.{coordinates[3]}";
            }
            else
            {
                latitude = $"{coordinates[0]}";
                longitude = $"{coordinates[1]}";
            }

            if (coordinates.Length >= 2)
            {

                if (float.TryParse(latitude.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float lat)
                    && float.TryParse(longitude.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float lon))
                {
                    project.formattedLocations.Add(new GeoCoordinate
                    {
                        latitude = lat,
                        longitude = lon
                    });
                }
            }
        }
    }

    private void TransformCountries(ProjectViewModel project)
    {
        if (string.IsNullOrEmpty(project.countries)) return;

        project.formattedCountries = new List<string>(project.countries.Split(new[] { ";" }, StringSplitOptions.None));
    }

    private void TransformInterventionAreas(ProjectViewModel project)
    {
        if (string.IsNullOrEmpty(project.intervention_areas)) return;

        project.formattedInterventionAreas = new List<string>(project.intervention_areas.Split(';'));
    }
    
    private void TransformRegions(ProjectViewModel project)
    {
        if (string.IsNullOrEmpty(project.regions)) return;

        project.formattedRegions = new List<string>(project.regions.Split(';'));
    }

    private string SerializeToIndentedJson<T>(T responseBody)
    {
        return JsonConvert.SerializeObject(responseBody, Formatting.Indented);
    }
}