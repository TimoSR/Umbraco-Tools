using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;

namespace WorldDiabetesFoundation.Core.Controllers.V1.Tags;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Tags")]
public class RegionController : ControllerBase
{
    private readonly RegionService _regionService;
    private readonly ILogger<ProjectsController> _logger;

    public RegionController(
        RegionService regionService, 
        ILogger<ProjectsController> logger
    )
    {
        _regionService = regionService;
        _logger = logger;
    }

    [HttpGet("Regions")]
    public IActionResult GetRegions()
    {
        return Ok(_regionService.GetRegions());
    }
}