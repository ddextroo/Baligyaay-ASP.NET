using Microsoft.AspNetCore.Http;

public class SessionManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ISession _session;

    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _session = _httpContextAccessor.HttpContext.Session;
    }

    public void SetSessionValue(string key, string value)
    {
        _session.SetString(key, value);
    }

    public string GetSessionValue(string key)
    {
        return _session.GetString(key);
    }

    // Additional methods for managing session values can be added here
}
