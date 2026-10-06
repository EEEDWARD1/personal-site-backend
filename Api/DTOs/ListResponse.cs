namespace Api.DTOs;

public record ListResponse<T>(IReadOnlyList<T> Items, int Total, int Limit, int Offset)
{
    public bool HasMore => Offset + Items.Count < Total;
}