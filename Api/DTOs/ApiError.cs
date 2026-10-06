namespace Api.DTOs;

public record ApiError(ErrorBody Error);
public record ErrorBody(string Code, string Message);