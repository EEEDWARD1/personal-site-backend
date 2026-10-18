namespace EduardTeodor.Api.Models;

public record PostListItem(
    string Slug,
    string Title,
    string Summary,
    bool Featured,
    IReadOnlyList<Tag> Tags);
