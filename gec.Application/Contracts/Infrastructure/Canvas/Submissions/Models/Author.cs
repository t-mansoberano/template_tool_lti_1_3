namespace gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

public class Author
{
    public long Id { get; set; }
    public string AnonymousId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarImageUrl { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public string? Pronouns { get; set; }    
}