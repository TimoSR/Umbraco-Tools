namespace WorldDiabetesFoundation.Web.Infrastructure.Integration;

public class ServiceError
{
    public string Message { get; }
    public int? StatusCode { get; }

    public ServiceError(string message, int? statusCode = null)
    {
        Message = message;
        StatusCode = statusCode;
    }
}