namespace gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;

public class SubmissionComment
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? Attempt { get; set; }
    public string AvatarPath { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public Author Author { get; set; } = new();    
}