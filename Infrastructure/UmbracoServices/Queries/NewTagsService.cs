using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;
using WorldDiabetesFoundation.Core.ViewModel;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;

public class NewTagsService
{
    private readonly ContentNodesExtractor _nodeExtractor;
    private readonly ILogger<NewTagsService> _logger;

    public NewTagsService(ContentNodesExtractor nodeExtractor, ILogger<NewTagsService> logger)
    {
        _nodeExtractor = nodeExtractor;
        _logger = logger;
    }

    public IEnumerable<IContent> GetRawTags()
    {
        var alias = Models.Tags.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetRawNodesByParentAlias(alias);

        return resultList;
    }
    
    public IEnumerable<TagsViewModel> GetAllTags()
    {
        var alias = Models.Tags.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetNodesByParentAlias<TagsViewModel>(alias);

        return resultList;
    }
}