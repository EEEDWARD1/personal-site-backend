using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EduardTeodor.Api.Authentication;

public static class ClerkAuthenticationExtensions
{
    public static IServiceCollection AddClerkAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var issuer = configuration["Clerk:Issuer"]
            ?? throw new InvalidOperationException("Clerk:Issuer must be configured.");
        if (!Uri.TryCreate(issuer, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Clerk:Issuer must be an HTTPS URL.");

        var adminUserId = configuration["Clerk:AdminUserId"];
        if (string.IsNullOrWhiteSpace(adminUserId))
            throw new InvalidOperationException("Clerk:AdminUserId must be configured.");

        var authorizedParties = configuration.GetSection("Clerk:AuthorizedParties").Get<string[]>() ?? [];
        if (authorizedParties.Length == 0)
            throw new InvalidOperationException("Clerk:AuthorizedParties must contain the CMS origin.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                    // Clerk's default session tokens have no aud claim. Validate azp below instead.
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var party = context.Principal?.FindFirst("azp")?.Value;
                        if (party is null || !authorizedParties.Contains(party, StringComparer.Ordinal)
                            || context.Principal?.FindFirst("sts")?.Value == "pending")
                            context.Fail("Invalid Clerk session.");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options => options.AddPolicy("Admin", policy =>
            policy.RequireAuthenticatedUser().RequireClaim("sub", adminUserId)));

        return services;
    }
}
