using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;

namespace WorldDiabetesFoundation.Core.Controllers.V1
{
    [ApiController]
    [Route("api/v1/")]
    [MapToApi("content-api")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Searchers")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> _logger;
        private readonly NewsSearcher _searcher;

        public NewsController(
            ILogger<NewsController> logger,
            NewsSearcher searcher)
        {
            _logger = logger;
            _searcher = searcher;
        }
        
        
        [HttpGet("News")]
        public IActionResult GetNews([FromQuery] Dictionary<string, string> input)
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
            
            var result = _searcher.SearchNews(input, generalSearch);
            
            return Ok(result);
        } 
    }
}

