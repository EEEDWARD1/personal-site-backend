using EduardTeodor.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EduardTeodor.Api.Health;

public sealed class DatabaseHealthCheck(AppDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Real, bounded queries check credentials, table access and mapped columns,
            // including on an empty database. A TCP/pg_isready check is not sufficient.
            await db.Posts.AsNoTracking().OrderBy(post => post.Id).Take(1).ToListAsync(cancellationToken);
            await db.Tags.AsNoTracking().OrderBy(tag => tag.Id).Take(1).ToListAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database readiness check failed.", ex);
        }
    }
}
