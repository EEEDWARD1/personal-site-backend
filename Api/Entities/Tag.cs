namespace Api.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";       // CITEXT
    public string Slug { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }

    public List<PostTag> PostTags { get; set; } = new();
}