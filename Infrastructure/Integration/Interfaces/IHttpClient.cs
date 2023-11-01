using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Core.Infrastructure.Integration.Interfaces;

public interface IHttpClient
{
    Task<IServiceResult> CallWebService(string requestUrl, IRequestBody requestBody);
    Task<IServiceResult> CallWebApiWithHeader(string requestUrl, string secretKey);
}