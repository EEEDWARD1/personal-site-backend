using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Repositories;
using EduardTeodor.Api.Service;

namespace EduardTeodor.Api.Tests;

public class PostServiceTests
{
    [Theory]
    [InlineData(0, 0, 1, 1, 0)]
    [InlineData(-1, 500, 1, 100, 0)]
    [InlineData(3, 20, 3, 20, 40)]
    [InlineData(int.MaxValue, 100, int.MaxValue, 100, int.MaxValue)]
    public async Task SummariesNormalizePagination(
        int page, int pageSize, int expectedPage, int expectedPageSize, int expectedSkip)
    {
        var repository = new StubPostRepository();
        var service = new PostService(repository);

        var result = await service.GetPublishedPostSummariesAsync(
            new PostQuery(null, null, null, Page: page, PageSize: pageSize), null);

        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedPageSize, result.PageSize);
        Assert.Equal(expectedSkip, repository.Skip);
        Assert.Equal(expectedPageSize, repository.Take);
        Assert.Equal(12, result.TotalCount);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task SummariesMapFieldsAndOnlyIncludeTagsWhenRequested(bool? enableTags)
    {
        var repository = new StubPostRepository();
        var service = new PostService(repository);

        var result = await service.GetPublishedPostSummariesAsync(
            new PostQuery(null, null, enableTags), null);

        var item = Assert.Single(result.Items);
        Assert.Equal("post-slug", item.Slug);
        Assert.Equal("Post title", item.Title);
        Assert.Equal("Post summary", item.Summary);
        Assert.True(item.Featured);
        Assert.Equal(enableTags is true, repository.IncludeTags);
        if (enableTags is true)
        {
            var tag = Assert.Single(item.Tags!);
            Assert.Equal("C#", tag.Name);
            Assert.Equal("csharp", tag.Slug);
        }
        else
        {
            Assert.Null(item.Tags);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TitlesMapFieldsAndRespectPaginationAndTags(bool enableTags)
    {
        var repository = new StubPostRepository();
        var service = new PostService(repository);

        var result = await service.GetPublishedPostTitlesAsync(
            new PostQuery(null, null, enableTags, Page: 2, PageSize: 10), null);

        var item = Assert.Single(result.Items);
        Assert.Equal("post-slug", item.Slug);
        Assert.Equal("Post title", item.Title);
        Assert.True(item.Featured);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(10, repository.Skip);
        Assert.Equal(10, repository.Take);
        Assert.Equal(enableTags, repository.IncludeTags);
        if (enableTags)
            Assert.Equal("csharp", Assert.Single(item.Tags!).Slug);
        else
            Assert.Null(item.Tags);
    }

    [Fact]
    public async Task ServiceNormalizesTagSlugsAndPassesFiltersToRepository()
    {
        var repository = new StubPostRepository();
        var service = new PostService(repository);

        await service.GetPublishedPostSummariesAsync(
            new PostQuery("project", true, false, [" csharp ", "", " ", "csharp", "dotnet"]),
            PostType.Project);

        Assert.Equal(PostType.Project, repository.Type);
        Assert.True(repository.IsFeatured);
        Assert.Equal(new[] { "csharp", "dotnet" }, repository.TagSlugs);
    }

    [Fact]
    public async Task MissingPostReturnsNull()
    {
        var repository = new StubPostRepository();
        var service = new PostService(repository);

        var result = await service.GetPublishedPostBySlugAsync("missing");

        Assert.Null(result);
        Assert.Equal("missing", repository.Slug);
    }

    [Fact]
    public async Task PublishedPostMapsDetailsAndTags()
    {
        var post = new Post
        {
            Slug = "post-slug",
            Title = "Post title",
            Summary = "Post summary",
            Content = "Post content",
            Featured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-1),
            Tags = [new Tag { Name = "C#", Slug = "csharp" }]
        };
        var repository = new StubPostRepository { Post = post };
        var service = new PostService(repository);

        var result = await service.GetPublishedPostBySlugAsync(post.Slug);

        Assert.NotNull(result);
        Assert.Equal(post.Slug, result.Slug);
        Assert.Equal(post.Title, result.Title);
        Assert.Equal(post.Summary, result.Summary);
        Assert.Equal(post.Content, result.Content);
        Assert.Equal(post.PublishedAt.Value, result.PublishedAt);
        Assert.True(result.Featured);
        Assert.Equal("csharp", Assert.Single(result.Tags).Slug);
    }

    private sealed class StubPostRepository : IPostRepository
    {
        public Post? Post { get; init; }
        public string? Slug { get; private set; }
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public PostType? Type { get; private set; }
        public bool? IsFeatured { get; private set; }
        public string[] TagSlugs { get; private set; } = [];
        public bool IncludeTags { get; private set; }

        public Task<Post?> GetPublishedPostBySlugAsync(string slug)
        {
            Slug = slug;
            return Task.FromResult(Post);
        }

        public Task<(IReadOnlyList<PostListItem> Items, int TotalCount)> GetPublishedPageAsync(
            int skip, int take, PostType? type, bool? isFeatured, string[] tagSlugs, bool includeTags)
        {
            Skip = skip;
            Take = take;
            Type = type;
            IsFeatured = isFeatured;
            TagSlugs = tagSlugs;
            IncludeTags = includeTags;

            IReadOnlyList<PostListItem> items =
            [
                new("post-slug", "Post title", "Post summary", true,
                    [new Tag { Name = "C#", Slug = "csharp" }])
            ];
            return Task.FromResult((items, 12));
        }
    }
}
