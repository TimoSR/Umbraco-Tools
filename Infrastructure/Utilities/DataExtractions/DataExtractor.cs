using Microsoft.Extensions.Logging;
using StackExchange.Profiling.Internal;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using WorldDiabetesFoundation.Core.Controllers.V1;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;

public class DataExtractor
{
    private readonly ILogger<ImageApiController> _logger;
    private readonly IUmbracoContextFactory _contextFactory;
    private readonly IContentService _contentService;

    public DataExtractor(
        ILogger<ImageApiController> logger,
        IUmbracoContextFactory contextFactory,
        IContentService contentService)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _contentService = contentService;
    }
    
    public string? GetImageWithCropsById(int id)
    {
        using var context = _contextFactory.EnsureUmbracoContext();
        var content = context.UmbracoContext.Content.GetById(id);

        if (content == null)
            return null;

        _logger.LogInformation($"Content Type Alias: {content.ContentType.Alias}");

        var imageProperty = content.Value<MediaWithCrops>("image");

        if (imageProperty == null)
            return null;

        var focalPoint = imageProperty.LocalCrops.FocalPoint;

        var cropsData = imageProperty.LocalCrops.Crops.Select(crop => new
        {
            alias = crop.Alias,
            width = crop.Width,
            height = crop.Height,
            // Coordinates is the zoom
            coordinates = crop.Coordinates != null
                ? new
                {
                    x1 = crop.Coordinates.X1,
                    y1 = crop.Coordinates.Y1,
                    x2 = crop.Coordinates.X2,
                    y2 = crop.Coordinates.Y2
                }
                : null
        }).ToList();

        var result = new
        {
            id = imageProperty.Content.Key,
            name = imageProperty.Name, // Set your actual value here
            url = imageProperty.Content.Url(),
            extension = Path.GetExtension(imageProperty.Content.Url()).Trim('.'),
            properties = new { }, // Set your actual properties here
            focalPoint = new
            {
                X = focalPoint?.Left,
                Y = focalPoint?.Top
            },
            crops = cropsData
        };

        return result.ToJson();
    }
    
    public string GetUrlFromNodeId(int Id)
    {
        using (var context = _contextFactory.EnsureUmbracoContext())
        {
            var projectNode = FindNodeById(Id);

            if (projectNode == null)
            {
                _logger.LogWarning($"Project node with project_id {Id} not found.");
                return null;
            }

            var id = projectNode.Id;
            
            var content = context.UmbracoContext.Content.GetById(id);

            if (content == null)
                return null;

            _logger.LogInformation($"Content Type Alias: {content.ContentType.Alias}");

            var Url = content.Url(mode: UrlMode.Absolute);

            return Url;
        }
    }

    public IContent FindParentNode(int id)
    {
        var rootNode = GetRootNode();
        var node = FindNodeById(id, rootNode);
        var parentNode = _contentService.GetParent(node.Id);
        return parentNode;
    }
    
    private IContent FindNodeById(int id)
    {
        var rootNode = GetRootNode();
        return FindNodeById(id, rootNode);
    }

    private IContent FindNodeById(int id, IContent currentNode)
    {   
        if (currentNode.Id == id)
        {
            return currentNode;
        }

        var currentNodeChildren = _contentService.GetPagedChildren(currentNode.Id, 0, int.MaxValue, out _);

        foreach (var child in currentNodeChildren)
        {
            var result = FindNodeById(id, child);
            if (result != null)
            {
                return result;
            }
        }
        
        return null;
    }

    private IContent GetRootNode()
    {
        var rootNodes = _contentService.GetRootContent();
        var node = rootNodes.FirstOrDefault(node => node.ContentType.Alias == Root.ModelTypeAlias);
        return node;
    }
}