using System.Globalization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities;
using WorldDiabetesFoundation.Core.ViewModel;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;

public class NewsSearcher
{
    private readonly ExamineSearcher _examineSearcher;
    private readonly DataExtractor _dataExtractor;
    private readonly ILogger<NewsSearcher> _logger;

    public NewsSearcher(
        ILogger<NewsSearcher> logger,
        ExamineSearcher examineSearcher,
        DataExtractor dataExtractor)
    {
        _logger = logger;
        _examineSearcher = examineSearcher;
        _dataExtractor = dataExtractor;
    }
    
    public string SearchNews(Dictionary<string, string> filters, string generalSearch)
    {
        HashSet<string>  specificFields = new() { "author", "imageDescription" };
        
        var fieldsToSearch = DefaultSearchFields.MergeFieldsWithDefault(specificFields);

        string searchResult = _examineSearcher.SearchByMultipleFields<NewsViewModel>(NewsPage.ModelTypeAlias, fieldsToSearch, filters, generalSearch);

        var transformedResult = TransformProjectResults(searchResult);

        transformedResult = DateProcessor(filters, transformedResult);

        var serializedObject = SerializeToIndentedJson(transformedResult);  // Assuming you have this method defined

        var projectJsonArray = JArray.Parse(serializedObject);

        return projectJsonArray.ToString();
    }

    private List<NewsViewModel> DateProcessor(Dictionary<string, string> filters, List<NewsViewModel> transformedResult)
    {
        const string fromDate = "fromDate";
        const string toDate = "toDate";
        
        DateTime? parsedFromDate = null;
        DateTime? parsedToDate = null;

        if (filters.ContainsKey(fromDate))
        {
            var fromDateValue = filters[fromDate];
            parsedFromDate = InputStringToDateTime(fromDateValue);
        }

        if (filters.ContainsKey(toDate))
        {
            var toDateValue = filters[toDate];
            parsedToDate = InputStringToDateTime(toDateValue);
        }

        if (parsedFromDate.HasValue || parsedToDate.HasValue)
        {
            transformedResult = LimitListByDateRange(transformedResult, parsedFromDate, parsedToDate);
        }

        transformedResult = SortListByLatestDate(transformedResult);

        return transformedResult;
    }

    private List<NewsViewModel> SortListByLatestDate(List<NewsViewModel> list)
    {
        list = list.OrderByDescending(news => UmbracoDateToDateTime(news.date)).ToList();
        return list;
    }

    private List<NewsViewModel> LimitListByDateRange(List<NewsViewModel> list, DateTime? fromDate, DateTime? toDate)
    {
        list.RemoveAll(element =>
        {
            var newsDate = UmbracoDateToDateTime(element.date);

            if (fromDate.HasValue && newsDate < fromDate.Value)
            {
                return true;
            }

            if (toDate.HasValue && newsDate > toDate.Value)
            {
                return true;
            }

            return false;
        });

        return list;
    }
    
    private static DateTime InputStringToDateTime(string dateString)
    {
        string format = "dd/MM/yyyy";
        CultureInfo provider = CultureInfo.InvariantCulture;
        DateTime parsedDate = DateTime.ParseExact(dateString, format, provider);
        DateTime dateOnly = parsedDate.Date;
        return dateOnly;
    }
    
    private DateTime UmbracoDateToDateTime(string dateString)
    {
        // Validate input
        if (string.IsNullOrEmpty(dateString))
        {
            throw new ArgumentException("Date string cannot be null or empty.", nameof(dateString));
        }

        // Remove AM and PM from the date string
        dateString = dateString.Replace(" AM", "").Replace(" PM", "");

        // Prepare the list of possible formats
        string[] formats = new string[]
        {
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH.mm.ss",
            "MM/dd/yyyy HH.mm.ss"
        };

        // Declare a CultureInfo provider
        CultureInfo provider = CultureInfo.InvariantCulture;

        DateTime parsedDate;

        // Try to parse the date string
        if (DateTime.TryParseExact(dateString, formats, provider, DateTimeStyles.None, out parsedDate))
        {
            // Retrieve the date part of the DateTime object
            DateTime dateOnly = parsedDate.Date;
            return dateOnly;
        }
        else
        {
            // Handle failure
            _logger.LogWarning("Unable to convert {dateString} to a DateTime object.", dateString);
            return DateTime.MinValue;
        }
    }
    
    private List<NewsViewModel> TransformProjectResults(string serializedResults)
    {
        var projectList = JsonConvert.DeserializeObject<List<NewsViewModel>>(serializedResults);
        
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