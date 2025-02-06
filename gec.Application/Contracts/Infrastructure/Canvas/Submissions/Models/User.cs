namespace gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string SortableName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string SisUserId { get; set; } = string.Empty;
    public string? IntegrationId { get; set; }
    public string? SisImportId { get; set; }
    public string LoginId { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}