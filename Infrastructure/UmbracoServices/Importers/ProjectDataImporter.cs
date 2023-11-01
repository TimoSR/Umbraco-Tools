using System.Globalization;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using WorldDiabetesFoundation.Core.DataTransferObjects;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools;
using WorldDiabetesFoundation.Models;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Importers;

public class ProjectDataImporter
{
    private readonly IContentService _contentService;
    private readonly ILogger<ProjectDataImporter> _logger;

    public ProjectDataImporter(IContentService contentService, ILogger<ProjectDataImporter> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }
    
    public void ProcessAndStoreProjects<TData>(IEnumerable<TData> entities) where TData : ProjectDTO
    {
        _logger.LogInformation("Starting to process and store projects.");

        try
        {
            var rootNode = GetRootNode();
            var projectsParentNode = FindNodeByAlias(rootNode, Models.ProjectContainerPage.ModelTypeAlias);
            var regionsParentNode = FindNodeByAlias(rootNode, Models.Regions.ModelTypeAlias);
            var focusAreaParentNode = FindNodeByAlias(rootNode, Models.FocusAreas.ModelTypeAlias);
            var statusParentNode = FindNodeByAlias(rootNode, Models.ProjectStatuses.ModelTypeAlias);

            // Initialize a HashSet to store existing region nodes.
            HashSet<string> existingRegionNodes = new HashSet<string>(
                _contentService.GetPagedChildren(regionsParentNode.Id, 0, int.MaxValue, out _)
                    .Select(n => n.GetValue<string>("value"))
            );
            
            HashSet<string> existingAreaNodes = new HashSet<string>(
                _contentService.GetPagedChildren(focusAreaParentNode.Id, 0, int.MaxValue, out _)
                    .Select(n => n.GetValue<string>("value"))
            );
            
            HashSet<string> existingStatusNodes = new HashSet<string>(
                _contentService.GetPagedChildren(statusParentNode.Id, 0, int.MaxValue, out _)
                    .Select(n => n.GetValue<string>("value"))
            );

            if (projectsParentNode == null)
            {
                _logger.LogWarning("Parent node for imported projects was not found!");
            }
            else
            {
                foreach (var entity in entities)
                {
                    ProcessStoreRegions(entity, regionsParentNode, existingRegionNodes);
                    ProcessStoreInterventionAreas(entity, focusAreaParentNode,  existingAreaNodes);
                    ProcessStoreStatus(entity, statusParentNode, existingStatusNodes);
                    FormatConverter.ConvertToSnakeCaseProjectDto(entity);
                    UpsertInContentTree(entity, projectsParentNode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing and storing projects.");
        }
    }
    
    // New method to publish and release all projects
    public void PublishAndReleaseAllProjects()
    {
        _logger.LogInformation("Starting to publish and release all projects.");

        try
        {
            var rootNode = GetRootNode();
            var projectsParentNode = rootNode != null ? FindNodeByAlias(rootNode, ProjectContainerPage.ModelTypeAlias) : null;

            if (projectsParentNode == null)
            {
                _logger.LogWarning("Parent node for imported projects was not found!");
                return;
            }

            var projectNodes = _contentService.GetPagedChildren(projectsParentNode.Id, 0, int.MaxValue, out _);

            foreach (var projectNode in projectNodes)
            {
                if (!projectNode.Published)
                {
                    _contentService.SaveAndPublish(projectNode);
                    _logger.LogInformation($"Project with ID {projectNode.Id} is now published.");
                }
            }

            _logger.LogInformation("Finished publishing and releasing all projects.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while publishing and releasing projects.");
        }
    }

    private void ProcessStoreRegions<TData>(TData entity, IContent parentNode, HashSet<string> existingRegionNodes) where TData : ProjectDTO
    {

        if (string.IsNullOrEmpty(entity.regions))
        {
            return;
        }
        
        try
        {
            if (parentNode == null)
            {
                _logger.LogWarning("Parent node for imported regions was not found!");
                return;
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var regions = entity.regions.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (var region in regions)
            {
                string formattedRegion = region.ToLower().Replace(" ", "_");
                if (!existingRegionNodes.Contains(formattedRegion))
                {
                    var regionNode = _contentService.CreateContent(textInfo.ToTitleCase(region), parentNode.GetUdi(), Region.ModelTypeAlias);
                    regionNode.SetValue("value", formattedRegion);
                    _contentService.SaveAndPublish(regionNode);
                    existingRegionNodes.Add(formattedRegion); // Add to HashSet
                }
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing and storing regions.");
        }
    }
    
    private void ProcessStoreStatus<TData>(TData entity, IContent parentNode, HashSet<string> existingStatusNodes) where TData : ProjectDTO
    {

        if (string.IsNullOrEmpty(entity.status))
        {
            return;
        }
        
        try
        {
            if (parentNode == null)
            {
                _logger.LogWarning("Parent node for imported regions was not found!");
                return;
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var statuses = entity.status.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (var status in statuses)
            {
                string formattedRegion = status.ToLower().Replace(" ", "_");
                if (!existingStatusNodes.Contains(formattedRegion))
                {
                    var regionNode = _contentService.CreateContent(textInfo.ToTitleCase(status), parentNode.GetUdi(), ProjectStatus.ModelTypeAlias);
                    regionNode.SetValue("Title", formattedRegion);
                    _contentService.SaveAndPublish(regionNode);
                    existingStatusNodes.Add(formattedRegion); // Add to HashSet
                }
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing and storing regions.");
        }
    }

    private void ProcessStoreInterventionAreas<TData>(TData entity, IContent parentNode, HashSet<string> existingRegionNodes) where TData : ProjectDTO
    {
        
        if (string.IsNullOrEmpty(entity.intervention_areas))
        {
            return;
        }
        
        try
        {
            if (parentNode == null)
            {
                _logger.LogWarning("Parent node for imported regions was not found!");
                return;
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var areas = entity.intervention_areas.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (var area in areas)
            {
                string formattedRegion = area.ToLower().Replace(" ", "_");
                if (!existingRegionNodes.Contains(formattedRegion))
                {
                    var areaNode = _contentService.CreateContent(textInfo.ToTitleCase(area), parentNode.GetUdi(), FocusArea.ModelTypeAlias);
                    areaNode.SetValue("value", formattedRegion);
                    _contentService.SaveAndPublish(areaNode);
                    existingRegionNodes.Add(formattedRegion); // Add to HashSet
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing and storing regions.");
        }
    }
    
    private IContent? FindNodeByAlias(IContent currentNode, string alias)
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

    private IContent? FindParentNodeByAlias(IContent? currentNode, string alias)
    {
        while (currentNode != null)
        {
            if (currentNode.ContentType.Alias == alias)
            {
                return currentNode;
            }
            currentNode = _contentService.GetById(currentNode.ParentId);
        }

        return null;
    }

    // Update or Insert = Upsert
    private void UpsertInContentTree<TData>(TData entity, IContent parentNode) where TData : ProjectDTO
    {
        var parentId = parentNode.Id;
        var parentAlias = parentNode.ContentType.Alias;

        _logger.LogInformation($"Starting upsert process for project number: {entity.project_id} under parent alias: {parentAlias}.");
        
        // Try to find an existing project node with the given project number
        var projectNode = _contentService.GetPagedChildren(parentId, 0, int.MaxValue, out _)
            .FirstOrDefault(projectNode => projectNode.GetValue<string>("project_id") == entity.project_id);

        // If the project doesn't exist, create a new one
        if (projectNode == null)
        {
            _logger.LogInformation($"Creating new project node for project number: {entity.project_id}.");
            var contentName = entity.project_id?? "No_Project_Number";
            projectNode = _contentService.CreateContent(contentName, parentNode.GetUdi(), ProjectPage.ModelTypeAlias);
        }
        else
        {
            _logger.LogInformation($"Found existing project node for project number: {entity.project_id}.");
        }
    
        // Compare and update the project node
        var nodeChanged = UpdateNodeIfChanged(entity, projectNode);

        // Save or save and publish based on its current state
        if (projectNode.Published && nodeChanged)
        {
            _contentService.SaveAndPublish(projectNode);
            _logger.LogInformation($"Project number: {entity.project_id} updated, saved and published.");
        }
        else if (nodeChanged)
        {
            _contentService.Save(projectNode);
            _logger.LogInformation($"Project number: {entity.project_id} updated and saved.");
        }
    }

    private bool UpdateNodeIfChanged(ProjectDTO newProjectData, IContent projectNode)
    {
        var nodeChanged = false;
        
        var currentData = new ProjectDTO();
        
        MapContentToProperties(projectNode, currentData);

        if (DifferencesExist(newProjectData, currentData))
        {

            nodeChanged = true;
            _logger.LogInformation($"Project Updated!");
            MapPropertiesToContent(newProjectData, projectNode);
        }
        else
        {
            _logger.LogInformation($"No Changes was Found. Update skipped!");
        }

        return nodeChanged;
    }
    
    private bool DifferencesExist(ProjectDTO project, ProjectDTO importedProject)
    {
        var properties = typeof(ProjectDTO).GetProperties();

        foreach (var property in properties)
        {
            // Ensure the property has a getter (is readable)
            if (property.CanRead)
            {
                var projectValue = property.GetValue(project);
                var importedProjectValue = property.GetValue(importedProject);

                // For simplicity, using object.Equals to check for equality
                // This works for most built-in types, but may require modification
                // for custom types or complex properties
                if (!Equals(projectValue, importedProjectValue))
                {
                    return true;  // Difference found
                }
            }
        }

        return false;  // No differences found
    }

    private void MapPropertiesToContent<TModel>(TModel entity, IContent content)
    {
        var properties = typeof(TModel).GetProperties();
        
        foreach (var property in properties)
        {
            var propertyName = property.Name;

            if (content.HasProperty(propertyName))
            {
                content.SetValue(propertyName, property.GetValue(entity)?.ToString());
            }
        }
    }

    private IEnumerable<TModel> MapContentListToProperties<TModel>(IEnumerable<IContent> contentList, TModel entity) where TModel : new()
    {

        var modelContainer = new TModel();
        
        var list = new List<TModel>();
        
        foreach (var content in contentList)
        {
            MapContentToProperties<TModel>(content, modelContainer);
            list.Add(modelContainer);
        }

        return list;
    }

    private void MapContentToProperties<TModel>(IContent content, TModel entity)
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