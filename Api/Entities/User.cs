namespace Api.Entities;

public class User
{
    public Guid Id { get; set; }                 // UUID PRIMARY KEY
    public Guid KeycloakUserId { get; set; }     // UUID UNIQUE NOT NULL
    public string? Username { get; set; }        // VARCHAR(50), nullable
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
