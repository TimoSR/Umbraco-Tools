using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

namespace WorldDiabetesFoundation.Core.Controllers.V1.RawData;

[ApiController]
[Route("api/v1/[controller]")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "RawData")]
public class RawDataController : ControllerBase
{
    private readonly FocusAreasService _focusAreas;
    private readonly RegionService _regions;
    private readonly ProjectService _projects;
    private readonly NewTagsService _newTags;
    private readonly ILogger<ProjectsController> _logger;

    public RawDataController(
        FocusAreasService focusAreas,
        RegionService regions,
        ProjectService projects,
        NewTagsService newTags,
        ILogger<ProjectsController> logger
    )
    {
        _focusAreas = focusAreas;
        _regions = regions;
        _projects = projects;
        _newTags = newTags;
        _logger = logger;
    }

    [HttpGet("FocusAreas")]
    public IActionResult FocusAreas()
    {
        return Ok(_focusAreas.GetRawFocusAreas());
    }
    
    [HttpGet("Tags")]
    public IActionResult Tags()
    {
        return Ok(_newTags.GetRawTags());
    }
    
    [HttpGet("Regions")]
    public IActionResult Regions()
    {
        return Ok(_regions.GetRawRegions());
    }
    
    [HttpGet("Projects")]
    public IActionResult Projects()
    {
        return Ok(_regions.GetRawRegions());
    }
}