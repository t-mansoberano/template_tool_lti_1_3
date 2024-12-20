namespace gec.Infrastructure.Canvas.Models;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SortableName { get; set; }
    public string ShortName { get; set; }
    public string SisUserId { get; set; }
    public string IntegrationId { get; set; }
    public string LoginId { get; set; }
}
