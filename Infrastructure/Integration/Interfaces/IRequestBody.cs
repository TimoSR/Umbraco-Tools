using Newtonsoft.Json;

namespace WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

public interface IRequestBody
{
    [JsonProperty("app")]
    string AppID { get; }
    
    [JsonProperty("app_secret")]
    string AppSecret { get; }
    
    [JsonProperty("client_id")]
    string ClientID { get; }
    
    [JsonProperty("client_secret")]
    string ClientSecret { get; }
}