using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorldDiabetesFoundation.Core.Infrastructure.Integration.Interfaces;
using WorldDiabetesFoundation.Web.Infrastructure.Integration;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Core.Infrastructure.Integration;

public class ApiDataFetcher : IHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiDataFetcher> _logger;

    public ApiDataFetcher(IHttpClientFactory httpClientFactory, ILogger<ApiDataFetcher> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger;
    }

    public async Task<IServiceResult> CallWebApiWithHeader(string requestUrl, string secretKey)
    {   
        using var client = _httpClientFactory.CreateClient(nameof(ApiDataFetcher));
        client.DefaultRequestHeaders.Add("secretkey", secretKey);
        
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            
            var response = await client.SendAsync(request);

            var statusCode = (int)response.StatusCode;
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(responseBody);
                return new ServiceResult(responseBody);
            }

            string errorMessage = statusCode switch
            {
                404 => $"HTTP Status Code: {statusCode} - The Case service was not found.",
                500 => $"HTTP Status Code: {statusCode} - The Case service returned an internal server error. The error message was: {response.ReasonPhrase}.",
                _ => $"The Case service responded with status Code: {statusCode}."
            };

            return new ServiceResult(new ServiceError(errorMessage, statusCode));
        }
        catch (Exception ex)
        {
            var errorMessage = $"An unhandled exception occurred while calling the Case service. The exception message was: {ex.Message}";
            return new ServiceResult(new ServiceError(errorMessage));
        }
        
    }
    
    public async Task<IServiceResult> CallWebService(string requestUrl, IRequestBody requestBody)
    {
        using var client = _httpClientFactory.CreateClient(nameof(ApiDataFetcher));

        var jsonPayload = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(requestUrl, jsonPayload);
            var statusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return new ServiceResult(responseBody);
            }

            string errorMessage = statusCode switch
            {
                404 => $"HTTP Status Code: {statusCode} - The Case service was not found.",
                500 => $"HTTP Status Code: {statusCode} - The Case service returned an internal server error. The error message was: {response.ReasonPhrase}.",
                _ => $"The Case service responded with status Code: {statusCode}."
            };

            return new ServiceResult(new ServiceError(errorMessage, statusCode));
        }
        catch (Exception ex)
        {
            var errorMessage = $"An unhandled exception occurred while calling the Case service. The exception message was: {ex.Message}";
            return new ServiceResult(new ServiceError(errorMessage));
        }
    }
}