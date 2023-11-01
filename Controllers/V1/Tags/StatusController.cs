using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

namespace WorldDiabetesFoundation.Core.Controllers.V1.Tags;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Tags")]
public class StatusController : ControllerBase
{
    private readonly StatusService _statusService;
    private readonly ILogger<StatusController> _logger;

    public StatusController(
        StatusService statusService, 
        ILogger<StatusController> logger
    )
    {
        _statusService = statusService;
        _logger = logger;
    }

    [HttpGet("Statuses")]
    public IActionResult GetStatusTags()
    {
        return Ok(_statusService.GetProjectStatus());
    }
}