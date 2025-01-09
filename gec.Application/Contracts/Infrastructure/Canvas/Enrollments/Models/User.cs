namespace gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string SortableName { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string SisUserId { get; set; } = null!;
    public string IntegrationId { get; set; } = null!;
    public string LoginId { get; set; } = null!;
}