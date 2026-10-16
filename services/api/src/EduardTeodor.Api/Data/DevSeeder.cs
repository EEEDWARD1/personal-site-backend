using Bogus;
using EduardTeodor.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EduardTeodor.Api.Data;

public static class DevSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Idempotent: don't seed twice
        if (await db.Posts.AnyAsync()) return;

        var faker = new Faker("en_GB");

        Randomizer.Seed = new Random(42); // same fake data every time

        var tags = new[] { "dotnet", "postgres", "docker", "nextjs", "homelab" }
            .Select(n => new Tag { Name = n, Slug = n })
            .ToList();

        var posts = new Faker<Post>()
            .CustomInstantiator(f => new Post
            {
                Slug = f.Lorem.Slug(4),
                Title = f.Lorem.Sentence(5).TrimEnd('.'),
                Summary = string.Empty,
                Content = string.Join("\n\n", f.Lorem.Paragraphs(4)),
            })
            .RuleFor(p => p.Type, f => f.PickRandom<PostType>())
            .RuleFor(p => p.Summary, f => f.Lorem.Sentence(15))
            .RuleFor(p => p.Featured, f => f.Random.Bool(0.2f))
            // Mix of published, drafts and scheduled, to test your filter
            .RuleFor(p => p.PublishedAt, f => f.Random.WeightedRandom(
                [f.Date.Past(1).ToUniversalTime(), (DateTime?)null, f.Date.Soon(14).ToUniversalTime()],
                [0.7f, 0.2f, 0.1f]))
            .RuleFor(p => p.Tags, f => f.PickRandom(tags, f.Random.Int(1, 3)).ToList())
            .Generate(50);

        db.Posts.AddRange(posts);
        await db.SaveChangesAsync();
    }
}
