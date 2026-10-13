using Bogus;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Models;

namespace ResourceApi.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Posts.AnyAsync()) return;

        var faker = new Faker("en_GB");

        var tags = Enumerable.Range(1,10).Select(i => new Tag
        {
            Id = Guid.NewGuid(),
            Name = $"Tag {i}",
            Slug = $"tag-{i}",
            CreatedAt = DateTime.UtcNow
        }).ToList();

     var posts = Enumerable.Range(1, 1000)
            .Select(i =>
            {
                var createdAt = faker.Date.PastOffset(2).UtcDateTime;
                var published = faker.Random.Bool(0.8f);

                return new Post
                {
                    Id = Guid.NewGuid(),
                    Kind = i % 2 == 0
                        ? PostKind.Project
                        : PostKind.Thought,
                    Slug = $"post-{i}",
                    Title = faker.Lorem.Sentence(),
                    Summary = faker.Lorem.Paragraph(),
                    Content = faker.Lorem.Paragraphs(4),
                    Featured = faker.Random.Bool(0.2f),
                    Published = published,
                    PublishedAt = published ? createdAt : null,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt
                };
            })
            .ToList();

        // Generate random post-tag relationships
        var postTags = posts
            .SelectMany(post => faker.Random
                .Shuffle(tags)
                .Take(faker.Random.Int(1, 3))
                .Select(tag => new PostTag
                {
                    PostId = post.Id,
                    TagId = tag.Id,
                    Post = post,
                    Tag = tag
                }))
            .ToList();

        // Save all entities
        db.Tags.AddRange(tags);
        db.Posts.AddRange(posts);
        db.PostTags.AddRange(postTags);

        await db.SaveChangesAsync();
    }
}