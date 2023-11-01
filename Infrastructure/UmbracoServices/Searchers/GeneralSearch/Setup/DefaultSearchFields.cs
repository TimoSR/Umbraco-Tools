namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;

public static class DefaultSearchFields
{
    public static readonly HashSet<string> DefaultFieldsToSearch = new() { "title", "description", "contentGrid" };
    
    public static HashSet<string> MergeFieldsWithDefault(IEnumerable<string> specificFields)
    {
        var mergedFields = new HashSet<string>(DefaultFieldsToSearch);
        foreach (var field in specificFields)
        {
            mergedFields.Add(field);
        }
        return mergedFields;
    }
}