namespace Api.Entities;

public class Post
{
    public Guid Id { get; set; }
    public PostKind Kind { get; set; }
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Summary { get; set; }         // nullable in SQL -> string?
    public string Content { get; set; } = "";
    public bool Featured { get; set; }
    public bool Published { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<PostTag> PostTags { get; set; } = new();
}