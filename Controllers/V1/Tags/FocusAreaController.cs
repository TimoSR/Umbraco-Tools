using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

namespace WorldDiabetesFoundation.Core.Controllers.V1.Tags;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Tags")]
public class FocusAreaController : ControllerBase
{
    private readonly FocusAreasService _service;
    private readonly ILogger<ProjectsController> _logger;

    public FocusAreaController(
        FocusAreasService service, 
        ILogger<ProjectsController> logger
    )
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet("FocusAreas")]
    public IActionResult focusAreas()
    {
        return Ok(_service.GetFocusAreas());
    }
}