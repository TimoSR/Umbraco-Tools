using WorldDiabetesFoundation.Web.Infrastructure.Integration;
using WorldDiabetesFoundation.Web.Infrastructure.Integration.Enum;

namespace WorldDiabetesFoundation.Core.Infrastructure.Integration.Interfaces;

public interface IServiceResult
{
    ResultStatus Status { get; }
    string Value { get; }
    ServiceError Error { get; }
}