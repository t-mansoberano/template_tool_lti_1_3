namespace gec.Application.Contracts.Infrastructure.Federation.Models;

public class FederetionContext
{
    public User User { get; set; } = new();
}

public class User
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserId { get; set; } = "";
    public bool IsInstructor { get; set; }
    public bool IsStudent { get; set; }
    public bool IsExternalCollaborator { get; set; }
    public bool IsWithoutRole { get; set; }
    public string Picture { get; set; } = "";
}