namespace ResourceApi.Models;

public class Post
{
    public Guid Id {get;set;}
    public PostKind Kind {get;set;}
    public required string Slug {get;set;}
    public required string Title {get;set;}

    public string? Summary {get;set;}
    public required string Content {get;set;}

    public bool Featured {get;set;}
    public bool Published {get;set;}

    public DateTime? PublishedAt{get;set;}

    public DateTime CreatedAt{get;set;}
    public DateTime UpdatedAt{get;set;}

    public ICollection<PostTag> PostTags { get; set; } = [];
}