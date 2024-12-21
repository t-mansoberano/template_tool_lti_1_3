using System.Text.Json;

namespace gec.Server.Common;

public class SessionStorageService : ISessionStorageService
{
    private readonly ISession _session;

    public SessionStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _session = httpContextAccessor.HttpContext?.Session 
                   ?? throw new InvalidOperationException("Session is not available.");
    }

    public void Store<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key)) 
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        var json = JsonSerializer.Serialize(value);
        _session.SetString(key, json);
    }

    public T? Retrieve<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) 
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        var json = _session.GetString(key);
        if (string.IsNullOrEmpty(json)) return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public void Remove(string key)
    {
        if (string.IsNullOrEmpty(key)) 
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        _session.Remove(key);
    }
}