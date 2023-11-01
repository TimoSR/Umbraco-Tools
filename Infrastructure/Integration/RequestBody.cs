using Newtonsoft.Json;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Web.Infrastructure.Integration;

public class RequestBody : IRequestBody
{
    [JsonProperty("app_id")]
    public string AppID { get; }
    
    [JsonProperty("app_secret")]
    public string AppSecret { get; }
    
    [JsonProperty("client_id")]
    public string ClientID { get; }
    
    [JsonProperty("client_secret")]
   public string ClientSecret { get; }

    public RequestBody(string appId, string appSecret, string clientID, string clientSecret)
    {
        AppID = appId;
        AppSecret = appSecret;
        ClientID = clientID;
        ClientSecret = clientSecret;
    }
}