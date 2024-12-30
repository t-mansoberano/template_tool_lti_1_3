namespace gec.Application.Contracts.Infrastructure.Lti.Models;

public class LtiContext
{
    public User User { get; set; } = new();
    public Course Course { get; set; } = new();
}

public class User
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserId { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    public string Picture { get; set; } = "";
}

public class Course
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Type { get; set; } = new();
}