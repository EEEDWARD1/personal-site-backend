namespace ResourceApi.Models;

public class Tag
{
    public Guid Id {get;set;}
    public required string Name {get;set;}
    public required string Slug {get;set;}

    public DateTime CreatedAt {get;set;}

    public ICollection<PostTag> PostTags {get;set;} = [];
}