using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;

namespace WorldDiabetesFoundation.Core.Controllers.V1;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Searchers")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectSearcher _projectSearcher;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        ProjectSearcher projectSearcher, 
        ILogger<ProjectsController> logger
        )
    {
        _projectSearcher = projectSearcher;
        _logger = logger;
    }

    [HttpGet("Projects")]
    public IActionResult GetProjects([FromQuery] Dictionary<string, string> input)
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
        var result = _projectSearcher.SearchProjects(input, generalSearch);
        return Ok(result);
    }
}