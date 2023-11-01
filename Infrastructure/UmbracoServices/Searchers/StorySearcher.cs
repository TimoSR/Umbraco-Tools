using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities;
using WorldDiabetesFoundation.Core.ViewModel;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;

public class StorySearcher
{
    private readonly ExamineSearcher _examineSearcher;
    private readonly DataExtractor _dataExtractor;

    public StorySearcher(
        ExamineSearcher examineSearcher,
        DataExtractor dataExtractor)
    {
        _examineSearcher = examineSearcher;
        _dataExtractor = dataExtractor;
    }

    public string SearchStories(Dictionary<string, string> filters, string generalSearch)
    {
        //Custom Search Settings
        HashSet<string>  specificFields = new() {};
        
        var fieldsToSearch = DefaultSearchFields.MergeFieldsWithDefault(specificFields);

        string searchResult = _examineSearcher.SearchByMultipleFields<StoryViewModel>(StoryPage.ModelTypeAlias, fieldsToSearch, filters, generalSearch);

        var transformedResult = TransformProjectResults<StoryViewModel>(searchResult);
        
        var serializedObject = SerializeToIndentedJson(transformedResult);
        
        var projectJsonArray = JArray.Parse(serializedObject);

        return projectJsonArray.ToString();
    }
    
    private List<TModel> TransformProjectResults<TModel>(string serializedResults) where TModel : StoryViewModel
    {
        var projectList = JsonConvert.DeserializeObject<List<TModel>>(serializedResults);
        if (projectList == null)
        {
            throw new JsonSerializationException("Failed to deserialize the project list.");
        }

        try
        {
            foreach (var project in projectList)
            {
                var image =_dataExtractor.GetImageWithCropsById(project.id);

                if (image != null)
                {
                    // Deserialize the image string into an IndentedImageData object
                    var deserializedImage = JsonConvert.DeserializeObject<ImageData>(image);
                    // Replace the original image string with the indented JSON string
                    project.image = deserializedImage;
                }

                project.link = _dataExtractor.GetUrlFromNodeId(project.id);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return projectList;
    }

    private string SerializeToIndentedJson<T>(T responseBody)
    {
        return JsonConvert.SerializeObject(responseBody, Formatting.Indented);
    }
}