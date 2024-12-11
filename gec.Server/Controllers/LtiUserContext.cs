namespace gec.Server.Controllers;

public class LtiUserContext
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public List<KeyValuePair<string, List<string>>> Context { get; set; }    
}