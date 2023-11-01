using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Core.Services;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

namespace WorldDiabetesFoundation.Core.Controllers.V1.Tags;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Tags")]
public class TagsController : ControllerBase
{
    private readonly NewTagsService _newTagsService;
    private readonly ILogger<TagService> _logger;

    public TagsController(
        NewTagsService newTagsService, 
        ILogger<TagService> logger
    )
    {
        _newTagsService = newTagsService;
        _logger = logger;
    }

    [HttpGet("GetAllTags")]
    public IActionResult GetTags()
    {
        return Ok(_newTagsService.GetAllTags());
    }
}