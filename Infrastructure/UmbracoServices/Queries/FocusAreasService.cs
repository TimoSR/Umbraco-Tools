using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;
using WorldDiabetesFoundation.Core.ViewModel;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

public class FocusAreasService
{
    private readonly ContentNodesExtractor _nodeExtractor;
    private readonly ILogger<FocusAreasService> _logger;

    public FocusAreasService(ContentNodesExtractor nodeExtractor, ILogger<FocusAreasService> logger)
    {
        _nodeExtractor = nodeExtractor;
        _logger = logger;
    }

    public IEnumerable<IContent> GetRawFocusAreas()
    {
        var alias = Models.FocusAreas.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetRawNodesByParentAlias(alias);

        return resultList;
    }
    
    public IEnumerable<FocusAreaViewModel> GetFocusAreas()
    {
        var alias = Models.FocusAreas.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetNodesByParentAlias<FocusAreaViewModel>(alias);

        return resultList;
    }
}