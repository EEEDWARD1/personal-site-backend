namespace EduardTeodor.Api.Models;

public class Tag
{
    public Guid Id { get; set; }                     // DB: uuidv7()
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public DateTime CreatedAt { get; set; }          // DB: now()

    public ICollection<Post> Posts { get; set; } = [];
}