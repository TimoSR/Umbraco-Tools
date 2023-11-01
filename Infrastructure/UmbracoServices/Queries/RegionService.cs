using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;
using WorldDiabetesFoundation.Core.ViewModel;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;

public class RegionService
{
    
    private readonly ContentNodesExtractor _nodeExtractor;
    private readonly ILogger<RegionService> _logger;

    public RegionService(ContentNodesExtractor nodeExtractor, ILogger<RegionService> logger)
    {
        _nodeExtractor = nodeExtractor;
        _logger = logger;
    }

    public IEnumerable<IContent> GetRawRegions()
    {
        var alias = Models.Regions.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetRawNodesByParentAlias(alias);

        return resultList;
    }
    
    public IEnumerable<RegionViewModel> GetRegions()
    {
        var alias = Models.Regions.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetNodesByParentAlias<RegionViewModel>(alias);

        return resultList;
    }
}