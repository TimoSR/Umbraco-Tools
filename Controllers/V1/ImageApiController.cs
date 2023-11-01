using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;
using WorldDiabetesFoundation.Core.ViewModel;

namespace WorldDiabetesFoundation.Core.Controllers.V1;

[ApiController]
[Route("api/v1/")]
[MapToApi("content-api")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Image")]
public class ImageApiController : ControllerBase
{
    private readonly ILogger<ImageApiController> _logger;
    private readonly DataExtractor _genericDataExtractor;

    public ImageApiController(
        ILogger<ImageApiController> logger,
        DataExtractor genericDataExtractor)
    {
        _logger = logger;
        _genericDataExtractor = genericDataExtractor;
    }
    
    [HttpGet("GetImageWithCropsById")]
    public ActionResult GetImageWithCropsById(int nodeId)
    {
        var image = _genericDataExtractor.GetImageWithCropsById(nodeId);

        if (image == null)
            return NotFound("Content not found");

        // Deserialize the image string into an IndentedImageData object
        var result = JsonConvert.DeserializeObject<ImageData>(image);
        // Replace the original image string with the indented JSON string

        return Ok(result);
    }
}