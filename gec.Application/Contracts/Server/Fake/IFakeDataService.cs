using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Server.Fake;

public interface IFakeDataService
{
    Result<T> GetFakeData<T>(string relativeJsonPath);
}
