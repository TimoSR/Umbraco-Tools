using WorldDiabetesFoundation.Core.Infrastructure.Integration.Interfaces;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Enum;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Interfaces;

namespace WorldDiabetesFoundation.Web.Infrastructure.Integration;

public class ServiceResult : IServiceResult
{
    public ResultStatus Status { get; }
    public string Value { get; }
    public ServiceError Error { get; }

    // Successful result
    public ServiceResult(string value)
    {
        Status = ResultStatus.Success;
        Value = value;
    }

    // Failed result
    public ServiceResult(ServiceError error)
    {
        Status = ResultStatus.Failure;
        Error = error;
    }
}