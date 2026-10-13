using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.DTOs;
using ResourceApi.Models;
using System.Text.RegularExpressions;

namespace ResourceApi.Repositories;

public class PostRepository(AppDbContext db) : IPostRepository
{
    public async Task<List<PostTitleDto>> GetPostTitlesAsync(bool? isProject, bool? isFeatured)
    {
        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.Published);

        if (isProject.HasValue) query = query.Where(p => p.IsProject == isProject.Value);

        if (isFeatured.HasValue) query = query.Where(p => p.Featured == isFeatured.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostTitleDto(
                p.Slug,
                p.Title,
                p.Featured
            )).ToListAsync();
    }

    public async Task<List<PostSummaryDto>> GetPostSummaryAsync(bool? isProject, bool? isFeatured)
    {
        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.Published);

        if (isProject.HasValue) query = query.Where(p => p.IsProject == isProject.Value);

        if (isFeatured.HasValue) query = query.Where(p => p.Featured == isFeatured.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostSummaryDto(
                p.Slug,
                p.Title,
                p.Summary,
                p.Featured
            )).ToListAsync();
    }

    public async Task<PostDetailedDto?> GetPostBySlugAsync(string slug)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Slug == slug && p.Published)
            .Select(p => new PostDetailedDto(
                p.Slug,
                p.Title,
                p.Summary,
                p.Content,
                p.Featured,
                p.CreatedAt,
                p.PostTags
                    .Select(pt => pt.Tag.Slug)
                    .ToList()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreatePostAsync(CreatePostDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Post title is required.");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Post content is required.");

        if (request.Tags is null)
            throw new ArgumentException("Tags must be provided.");

        var now = DateTime.UtcNow;

        // Generate a slug from the title.
        var slug = Regex.Replace(
            request.Title.Trim().ToLowerInvariant(),
            @"[^a-z0-9]+",
            "-"
        ).Trim('-');

        if (string.IsNullOrEmpty(slug))
            throw new ArgumentException("Title must contain letters or numbers.");

        // Ensure the slug is unique.
        var baseSlug = slug;
        var suffix = 2;

        while (await db.Posts.AnyAsync(p => p.Slug == slug))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        // Create the post entity.
        var post = new Post
        {
            Id = Guid.NewGuid(),
            IsProject = request.IsProject,
            Slug = slug,
            Title = request.Title.Trim(),
            Summary = request.Summary,
            Content = request.Content,
            Featured = request.Featured,
            Published = request.Published,
            PublishedAt = request.Published ? now : null,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Normalise the requested tag names.
        var tagNames = request.Tags
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Find existing tags.
        var existingTags = await db.Tags
            .Where(tag => tagNames.Contains(tag.Name))
            .ToListAsync();

        var tags = new List<Tag>(existingTags);

        // Create missing tags.
        foreach (var name in tagNames)
        {
            if (tags.Any(tag =>
                string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var tagSlug = Regex.Replace(
                name.ToLowerInvariant(),
                @"[^a-z0-9]+",
                "-"
            ).Trim('-');

            if (string.IsNullOrEmpty(tagSlug))
                throw new ArgumentException($"Invalid tag name: {name}");

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = name,
                Slug = tagSlug,
                CreatedAt = now
            };

            tags.Add(tag);
            db.Tags.Add(tag);
        }

        // Associate tags with the post.
        foreach (var tag in tags)
        {
            post.PostTags.Add(new PostTag
            {
                PostId = post.Id,
                TagId = tag.Id,
                Post = post,
                Tag = tag
            });
        }

        db.Posts.Add(post);

        await db.SaveChangesAsync();

        return post.Slug;
    }
}