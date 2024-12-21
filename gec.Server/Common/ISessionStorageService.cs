namespace gec.Server.Common;

public interface ISessionStorageService
{
    void Store<T>(string key, T value);
    T? Retrieve<T>(string key);
    void Remove(string key);
}