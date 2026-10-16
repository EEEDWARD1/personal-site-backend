using EduardTeodor.Api.Binding;

namespace EduardTeodor.Api.DTOs.Tags;

public record TagQuery(
    int Page = 1,
    int PageSize =20
);
