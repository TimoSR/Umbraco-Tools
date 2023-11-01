using System.Globalization;
using WorldDiabetesFoundation.Core.DataTransferObjects;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools;

public static class FormatConverter
{
    public static void ConvertToTitleCaseProjectDto(ProjectDTO originalProject)
    {
        originalProject.intervention_areas = ConvertToTitleCase(originalProject.intervention_areas);
        originalProject.regions = ConvertToTitleCase(originalProject.regions);
        originalProject.countries = ConvertToTitleCase(originalProject.countries);
    }

    public static void ConvertToSnakeCaseProjectDto(ProjectDTO originalProject)
    {
        // Copy data and format fields

        originalProject.intervention_areas = ConvertToSnakeCaseCommaSep(originalProject.intervention_areas);
        originalProject.regions = ConvertToSnakeCase(originalProject.regions);
        //originalProject.countries = ConvertToSnakeCase(originalProject.countries);
    }

    private static string ConvertToTitleCase(string stringOfWords)
    {   
        
        if (string.IsNullOrEmpty(stringOfWords))
        {
            return null;
        }
        
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        
        // Perform any necessary string formatting
        return string.Join(";", textInfo.ToTitleCase(stringOfWords));
    }

    private static string ConvertToSnakeCase(string stringOfWords)
    {   
        
        if (string.IsNullOrEmpty(stringOfWords))
        {
            return null;
        }
        
        return string.Join(";", stringOfWords.ToLower().Replace(" ", "_"));
    }
    
    private static string ConvertToSnakeCaseCommaSep(string stringOfWords)
    {
        
        if (string.IsNullOrEmpty(stringOfWords))
        {
            return null;
        }
        
        // Split the string by commas to handle each word group separately
        var wordGroups = stringOfWords.Split(",", StringSplitOptions.RemoveEmptyEntries);

        // Process each word group to convert spaces to underscores and lowercase the letters
        var processedGroups = wordGroups.Select(group => string.Join("_", group.Split(' ').Select(word => word.ToLower())));

        // Join the processed word groups back together with commas
        return string.Join(", ", processedGroups);
    }
}