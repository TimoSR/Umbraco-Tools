using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Core.Infrastructure;

public class ProjectSettings : IProjectSettings
{
    public string ApiUrl { get; set; }
    public string SecurityToken { get; set; }
    public string SecretKey { get; set; }
    public IRequestBody RequestBody { get; set; }
}