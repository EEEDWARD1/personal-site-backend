namespace EduardTeodor.Api.Models;

public class Post
{
    public Guid Id { get; set; }                     // DB: uuidv7()
    public PostType Type { get; set; } = PostType.Thought;
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public required string Content { get; set; }
    public bool Featured { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }          // DB: now()
    public DateTime UpdatedAt { get; set; }          // DB: trigger
    public uint Version { get; set; }               // PostgreSQL xmin concurrency token

    public ICollection<Tag> Tags { get; set; } = [];
}
