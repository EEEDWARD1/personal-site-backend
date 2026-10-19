using System.ComponentModel.DataAnnotations;

namespace EduardTeodor.Api.Validation;

public sealed class PostSlugAttribute : ValidationAttribute
{
    public PostSlugAttribute() : base("The slug 'titles' is reserved for the post titles endpoint.") { }

    public override bool IsValid(object? value) =>
        value is not string slug || !string.Equals(slug, "titles", StringComparison.OrdinalIgnoreCase);
}
