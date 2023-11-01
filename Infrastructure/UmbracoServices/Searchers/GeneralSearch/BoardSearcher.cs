using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities;
using WorldDiabetesFoundation.Core.ViewModel;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers.GeneralSearch;

public class BoardSearcher
{
    private readonly ExamineSearcher _examineSearcher;
    private readonly DataExtractor _dataExtractor;

    public BoardSearcher(
        ExamineSearcher examineSearcher,
        DataExtractor dataExtractor)
    {
        _examineSearcher = examineSearcher;
        _dataExtractor = dataExtractor;
    }

    public string Search(Dictionary<string, string> filters, string generalSearch)
    {
        HashSet<string>  specificFields = new() {};
        
        var fieldsToSearch = DefaultSearchFields.MergeFieldsWithDefault(specificFields);

        string searchResult = _examineSearcher.SearchByMultipleFields<DefaultGeneralSearchView>(StoryPage.ModelTypeAlias, fieldsToSearch, filters, generalSearch);

        var transformedResult = TransformProjectResults<DefaultGeneralSearchView>(searchResult);
        
        var serializedObject = SerializeToIndentedJson(transformedResult);
        
        var projectJsonArray = JArray.Parse(serializedObject);

        return projectJsonArray.ToString();
    }
    
    private List<TModel> TransformProjectResults<TModel>(string serializedResults) where TModel : DefaultGeneralSearchView
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