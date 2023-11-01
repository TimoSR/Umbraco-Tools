using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers.GeneralSearch.Setup;

namespace WorldDiabetesFoundation.Core.Controllers.V1.Searchers;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Searchers")]
public class GeneralSearchController : ControllerBase
{
    private readonly GeneralSearcher _searcher;
    private readonly ILogger<ProjectsController> _logger;

    public GeneralSearchController(
        GeneralSearcher searcher, 
        ILogger<ProjectsController> logger
    )
    {
        _searcher = searcher;
        _logger = logger;
    }

    [HttpGet("GeneralSearch")]
    public IActionResult GeneralSearch([FromQuery] Dictionary<string, string> input)
    {

        string generalSearch;
        
        _logger.LogInformation("trying to get query key");

        foreach (var key in input)
        {
            _logger.LogInformation($"{key.Key}");           
        }

        if (input.TryGetValue("query", out generalSearch))
        {
            
        }
        
        _logger.LogInformation($"Query Parameters: {string.Join(", ", input.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
        var result = _searcher.Search(input, generalSearch);
        return Ok(result);
    }
}