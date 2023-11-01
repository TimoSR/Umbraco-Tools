using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;

public class ContentNodesExtractor
{
    private readonly ILogger<ContentNodesExtractor> _logger;
    private readonly IContentService _contentService;
    
    public ContentNodesExtractor(IContentService contentService,ILogger<ContentNodesExtractor> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }
    
    public IEnumerable<TModel> GetNodesByParentAlias<TModel>(string parentAlias) where TModel : new()
    {   
        var parent = GetParentNode(parentAlias);
    
        if (parent == null)
        {
            _logger.LogWarning($"Parent node with alias '{parentAlias}' not found.");
            return Enumerable.Empty<TModel>();
        }

        _logger.LogDebug(parent.ContentType.ToString());
    
        var children = _contentService.GetPagedChildren(parent.Id, 0, int.MaxValue, out _);
    
        var json = JsonConvert.SerializeObject(children);
        var objects = JsonConvert.DeserializeObject<IEnumerable<TModel>>(json);

        // Use Zip to combine the two lists into a single list of pairs, 
        // and then iterate through the pairs.
        foreach (var (child, obj) in children.Zip(objects, (c, o) => (c, o)))
        {
            MapContentToProperties(child, obj);
        }

        return objects;
    }

    public IEnumerable<IContent> GetRawNodesByParentAlias(string alias)
    {
        var parent = GetParentNode(alias);
        
        _logger.LogDebug(parent.ContentType.ToString());
        
        var children = _contentService.GetPagedChildren(parent.Id, 0, int.MaxValue, out _);

        return children;
    }
    
    private IContent GetParentNode(string parentAlias)
    {
        var currentNode = GetRootNode();
        return FindNodeByAlias(currentNode, parentAlias);
    }
    
    private IContent FindNodeByAlias(IContent currentNode, string alias)
    {
        if (currentNode.ContentType.Alias == alias)
        {
            return currentNode;
        }

        var currentNodeChildren = _contentService.GetPagedChildren(currentNode.Id, 0, int.MaxValue, out _);

        foreach (var child in currentNodeChildren)
        {
            var result = FindNodeByAlias(child, alias);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
    
    public void MapContentToProperties<TModel>(IContent content, TModel entity)
    {
        var properties = typeof(TModel).GetProperties();

        foreach (var property in properties)
        {
            var propertyName = property.Name;

            if (content.HasProperty(propertyName))
            {
                var contentValue = content.GetValue(propertyName);
                
                if (contentValue != null && property.CanWrite)
                {
                    var convertedValue = Convert.ChangeType(contentValue, property.PropertyType);
                    
                    property.SetValue(entity, convertedValue);   
                }
            }
        }
    }
    
    private IContent GetRootNode()
    {
        var rootNodes = _contentService.GetRootContent();
        var node = rootNodes.FirstOrDefault(node => node.ContentType.Alias == Root.ModelTypeAlias);
        return node;
    }
}