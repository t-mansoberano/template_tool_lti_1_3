using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Server;

public interface ISessionStorageService
{
    Result Store<T>(string key, T value);
    Result<T> Retrieve<T>(string key);
    Result Remove(string key);
}