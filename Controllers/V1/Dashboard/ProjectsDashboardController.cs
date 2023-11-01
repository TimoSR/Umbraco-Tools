using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;

namespace WorldDiabetesFoundation.Core.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Project Dashboard")]
public class ProjectsDashboardController : ControllerBase
{
    private readonly ProjectService _service;
    
    public ProjectsDashboardController(ProjectService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public IActionResult GetAll()
    {
        var result = _service.GetProjects();
        return Ok(result);
    }
    
    [HttpGet("GetPublished")]
    public IActionResult GetPublished()
    {
        var result = _service.GetPublished();
        return Ok(result);
    }
    [HttpGet("GetUnpublished")]
    public IActionResult GetUnpublished()
    {
        var result = _service.GetUnpublished();
        return Ok(result);
    }
}