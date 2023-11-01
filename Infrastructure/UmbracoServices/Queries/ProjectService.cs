using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;
using WorldDiabetesFoundation.Core.ViewModel;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;

public class ProjectService
{
    private readonly ContentNodesExtractor _nodeExtractor;
    private readonly ILogger<FocusAreasService> _logger;

    public ProjectService(ContentNodesExtractor nodeExtractor, ILogger<FocusAreasService> logger)
    {
        _nodeExtractor = nodeExtractor;
        _logger = logger;
    }

    public IEnumerable<DashBoardViewModel> GetProjects()
    {
        var alias = Models.ProjectContainerPage.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetNodesByParentAlias<DashBoardViewModel>(alias);

        return resultList;
    }
    
    public IEnumerable<IContent> GetRawProjects()
    {
        var alias = Models.ProjectContainerPage.ModelTypeAlias;
        
        var resultList = _nodeExtractor.GetRawNodesByParentAlias(alias);

        return resultList;
    }

    public IEnumerable<DashBoardViewModel> GetPublished()
    {
        var projects = GetProjects();
        var publishedList = new List<DashBoardViewModel>();

        foreach (var project in projects)
        {
            if (project.Published)
            {
                publishedList.Add(project);
            }
        }

        return publishedList;
    }

    public IEnumerable<DashBoardViewModel> GetUnpublished()
    {
        var projects = GetProjects();
        var unpublishedList = new List<DashBoardViewModel>();
        foreach (var project in projects)
        {
            if(!project.Published)
            {
                unpublishedList.Add(project);
            }
        }
        return unpublishedList;
    }
}