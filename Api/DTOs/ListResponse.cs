namespace Api.DTOs;

public record ListResponse<T>(IReadOnlyList<T> Items);