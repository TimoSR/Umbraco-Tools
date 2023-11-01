using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;
using WorldDiabetesFoundation.Core.ViewModel;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

public class StatusService
{
    private readonly ContentNodesExtractor _nodeExtractor;
    private readonly ILogger<FocusAreasService> _logger;

    public StatusService(ContentNodesExtractor nodeExtractor, ILogger<FocusAreasService> logger)
    {
        _nodeExtractor = nodeExtractor;
        _logger = logger;
    }

    public IEnumerable<IContent> GetRawFocusAreas()
    {
        var alias = Models.ProjectStatuses.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetRawNodesByParentAlias(alias);

        return resultList;
    }
    
    public IEnumerable<StatusViewModel> GetProjectStatus()
    {
        var alias = Models.ProjectStatuses.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetNodesByParentAlias<StatusViewModel>(alias);

        return resultList;
    }
}