using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using EduardTeodor.Api.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace EduardTeodor.Api.Tests;

public class ClerkAuthenticationTests : IAsyncLifetime
{
    private const string Issuer = "https://clerk.example.test";
    private const string AdminId = "user_3KvGpJwyF1axbnsKPdCYemBhkh6";
    private const string Origin = "https://cms.eduardteodor.co.uk";
    private readonly RSA _rsa = RSA.Create(2048);
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Clerk:Issuer"] = Issuer,
            ["Clerk:AdminUserId"] = AdminId,
            ["Clerk:AuthorizedParties:0"] = Origin
        }).Build();
        builder.Services.AddClerkAuthentication(configuration);
        builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var oidc = new OpenIdConnectConfiguration { Issuer = Issuer };
            oidc.SigningKeys.Add(new RsaSecurityKey(_rsa) { KeyId = "test-key" });
            options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(oidc);
        });
        _app = builder.Build();
        _app.UseAuthentication();
        _app.UseAuthorization();
        _app.MapGet("/admin", () => "Admin content").RequireAuthorization("Admin");
        _app.MapGet("/public", () => "Public content");
        await _app.StartAsync();
        _client = new HttpClient { BaseAddress = new Uri(_app.Urls.Single()) };
    }

    private string Token(string? userId = AdminId, string? origin = Origin, string issuer = Issuer,
        bool expired = false, bool pending = false, SecurityKey? key = null, string algorithm = SecurityAlgorithms.RsaSha256)
    {
        var claims = new List<Claim>();
        if (userId is not null) claims.Add(new Claim("sub", userId));
        if (origin is not null) claims.Add(new Claim("azp", origin));
        if (pending) claims.Add(new Claim("sts", "pending"));
        var token = new JwtSecurityToken(issuer: issuer, claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: DateTime.UtcNow.AddMinutes(expired ? -5 : 5),
            signingCredentials: new SigningCredentials(key ?? new RsaSecurityKey(_rsa) { KeyId = "test-key" }, algorithm));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<HttpStatusCode> Request(string? token, string path = "/admin")
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await _client.SendAsync(request);
        return response.StatusCode;
    }

    [Fact]
    public async Task VerifiedOwnerIsAuthorizedWithoutARoleClaim() =>
        Assert.Equal(HttpStatusCode.OK, await Request(Token()));

    [Fact]
    public async Task OtherVerifiedUserIsForbidden() =>
        Assert.Equal(HttpStatusCode.Forbidden, await Request(Token(userId: "user_other")));

    [Fact]
    public async Task PreviousDevelopmentUserIsForbidden() =>
        Assert.Equal(HttpStatusCode.Forbidden, await Request(Token(userId: "user_3KvCdrD7lHa2pwct9Ou7gQbB4NP")));

    [Fact]
    public async Task MissingSubjectIsForbidden() =>
        Assert.Equal(HttpStatusCode.Forbidden, await Request(Token(userId: null)));

    [Fact]
    public async Task AnonymousIsUnauthorized() =>
        Assert.Equal(HttpStatusCode.Unauthorized, await Request(null));

    [Theory]
    [InlineData("http://evil.test")]
    [InlineData("http://localhost:3001")]
    [InlineData("http://localhost:3000")]
    [InlineData(null)]
    public async Task WrongOrMissingAuthorizedPartyIsRejected(string? origin) =>
        Assert.Equal(HttpStatusCode.Unauthorized, await Request(Token(origin: origin)));

    [Fact]
    public async Task WrongIssuerIsRejected() =>
        Assert.Equal(HttpStatusCode.Unauthorized, await Request(Token(issuer: "https://other.clerk.test")));

    [Fact]
    public async Task ExpiredTokenIsRejected() =>
        Assert.Equal(HttpStatusCode.Unauthorized, await Request(Token(expired: true)));

    [Fact]
    public async Task PendingSessionIsRejected() =>
        Assert.Equal(HttpStatusCode.Unauthorized, await Request(Token(pending: true)));

    [Fact]
    public async Task ForgedSigningKeyIsRejected()
    {
        using var other = RSA.Create(2048);
        Assert.Equal(HttpStatusCode.Unauthorized, await Request(Token(key: new RsaSecurityKey(other) { KeyId = "test-key" })));
    }

    [Fact]
    public async Task LegacySharedSecretTokensAreRejected()
    {
        var key = new SymmetricSecurityKey(new byte[64]);
        Assert.Equal(HttpStatusCode.Unauthorized,
            await Request(Token(key: key, algorithm: SecurityAlgorithms.HmacSha256)));
    }

    [Fact]
    public async Task PublicRoutesRemainAnonymous() =>
        Assert.Equal(HttpStatusCode.OK, await Request(null, "/public"));

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        _rsa.Dispose();
    }
}
