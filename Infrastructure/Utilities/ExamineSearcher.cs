using System.Reflection;
using Examine;
using Examine.Search;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools._Interface;

namespace WorldDiabetesFoundation.Core.Infrastructure.Utilities;

public class ExamineSearcher : ISearchService
{
    private readonly ILogger<ExamineSearcher> _logger;
    private readonly IExamineManager _examineManager;

    public ExamineSearcher(
        ILogger<ExamineSearcher> logger,
        IExamineManager examineManager)
    {
        _logger = logger;
        _examineManager = examineManager;
        _logger.LogInformation("ContentSearcher service has been initialized.");
    }

    public string SearchByMultipleFields<TModel>(
        string nodeTypeAlias,
        IEnumerable<string> fieldsToSearch,
        Dictionary<string, string> filters = null, 
        string generalSearch = null) where TModel : new()
    {
        _logger.LogInformation("Starting SearchByMultipleFields method.");
        try
        {
            var resultList = ExecuteSearchQueries<TModel>(nodeTypeAlias, fieldsToSearch, filters, generalSearch);
            var serializedResults = ConvertAndSerializeResults<TModel>(resultList);
            _logger.LogInformation("SearchByMultipleFields completed successfully.");
            return serializedResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while performing SearchByMultipleFields.");
            throw;
        }
    }
    
    private List<ISearchResult> ExecuteSearchQueries<TModel>(
        string nodeTypeAlias,
        IEnumerable<string> fieldsToSearch,
        Dictionary<string, string> filters = null, 
        string generalSearch = null) where TModel : new()
    {
        _logger.LogInformation("Starting ExecuteSearchQueries method.");
        var resultList = new List<ISearchResult>();
        var propertyMapping = GetModelPropertyNamesFromJsonNames<TModel>(filters);

        if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex? index))
        {
            _logger.LogWarning("ExternalIndex not found.");
            return resultList;
        }

        _logger.LogDebug("ExternalIndex found.");
        
        var searchQuery = index.Searcher.CreateQuery("content").NodeTypeAlias(nodeTypeAlias);
        var booleanQuery = ConstructBooleanQuery(searchQuery, fieldsToSearch, propertyMapping, generalSearch);
        var searchResults = booleanQuery.Execute();
        resultList.AddRange(searchResults);

        _logger.LogInformation("ExecuteSearchQueries method completed.");
        return resultList;
    }

    private IBooleanOperation ConstructBooleanQuery(
        IBooleanOperation searchQuery,
        IEnumerable<string> fieldsToSearch,
        Dictionary<string, string> propertyMapping, 
        string generalSearch)
    {
        IBooleanOperation booleanQuery = searchQuery;

        booleanQuery = HandleExactMatches(generalSearch, booleanQuery, fieldsToSearch);
        booleanQuery = HandleFieldSpecificSearch(propertyMapping, booleanQuery);
        booleanQuery = HandleGeneralSearch(generalSearch, booleanQuery, fieldsToSearch);

        return booleanQuery;
    }

    private IBooleanOperation HandleExactMatches(
        string generalSearch, 
        IBooleanOperation booleanQuery,
        IEnumerable<string> fieldsToSearch)
    {
        if (string.IsNullOrWhiteSpace(generalSearch)) return booleanQuery;

        var exactMatchQuery = $"\"{generalSearch}\"";
        var rawQuery = string.Join(" OR ", fieldsToSearch.Select(field => $"{field}:{exactMatchQuery}"));
        return booleanQuery.And().NativeQuery(rawQuery);
    }

    private IBooleanOperation HandleFieldSpecificSearch(Dictionary<string, string> propertyMapping, IBooleanOperation booleanQuery)
    {
        if (propertyMapping == null) return booleanQuery;

        foreach (var filter in propertyMapping)
        {
            booleanQuery = booleanQuery.And().Field(filter.Key, filter.Value);
        }
        return booleanQuery;
    }

    private IBooleanOperation HandleGeneralSearch(string generalSearch, IBooleanOperation booleanQuery, IEnumerable<string> fieldsToSearch)
    {
        if (string.IsNullOrWhiteSpace(generalSearch)) return booleanQuery;
        
        IBooleanOperation generalQuery = null;

        foreach (var field in fieldsToSearch)
        {
            generalQuery = generalQuery == null 
                ? booleanQuery.Or().Field(field, generalSearch) 
                : generalQuery.Or().Field(field, generalSearch);
        }

        return generalQuery ?? booleanQuery;
    }
    
    private Dictionary<string, string> GetModelPropertyNamesFromJsonNames<TModel>(Dictionary<string, string> jsonNames)
    {
        _logger.LogDebug("Mapping model properties to JSON names.");
        var mapping = new Dictionary<string, string>();
        var properties = typeof(TModel).GetProperties();
        foreach (var property in properties)
        {
            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty != null)
            {
                var jsonPropertyName = jsonProperty.PropertyName;
                if (jsonNames.ContainsKey(jsonPropertyName))
                {
                    mapping[jsonPropertyName] = jsonNames[jsonPropertyName]; // Use JSON property name here
                }
            }
        }
        return mapping;
    }
    
    private string ConvertAndSerializeResults<TModel>(IEnumerable<ISearchResult> resultList) where TModel : new()
    {        
        _logger.LogDebug("Converting and serializing results.");
        var convertedResult = new List<TModel>();
        
        foreach (var result in resultList)
        {
            _logger.LogInformation(SerializeToIndentedJson(result));
            var modelContainer = new TModel();
            MapContentToProperties(result, modelContainer);
            convertedResult.Add(modelContainer);
        }
    
        return SerializeToIndentedJson(convertedResult);
    }

    private void MapContentToProperties<TModel>(ISearchResult content, TModel entity)
    {
        _logger.LogDebug($"Mapping content to properties for type {typeof(TModel).Name}.");
        var properties = typeof(TModel).GetProperties();
        
        foreach (var property in properties)
        {
            var propertyName = property.Name;
    
            if (content.Values.ContainsKey(propertyName))
            {
                var propertyValue = content.Values[propertyName];
    
                if (propertyValue != null && property.CanWrite)
                {
                    Console.WriteLine(propertyValue);
                    
                    var convertedValue = Convert.ChangeType(propertyValue, property.PropertyType);
    
                    property.SetValue(entity, convertedValue);
                }
            }
        }
    }
    
    private string SerializeToIndentedJson<T>(T responseBody)
    {
        _logger.LogDebug("Serializing response body to indented JSON.");
        return JsonConvert.SerializeObject(responseBody, Formatting.Indented);
    }
}