using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Core.Infrastructure;

public interface IProjectSettings
{
    string ApiUrl { get; set; }
    string SecurityToken { get; set; }
    string SecretKey { get; set; }

    IRequestBody RequestBody { get; set; }
}