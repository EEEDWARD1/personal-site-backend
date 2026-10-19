using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Repositories;
using EduardTeodor.Api.Service;

namespace EduardTeodor.Api.Tests;

public class AdminPostServiceTests
{
    [Fact]
    public async Task GetByIdIncludesDraftDetailsReadOnlyMetadataAndVersion()
    {
        var post = new Post
        {
            Id = Guid.NewGuid(), Slug = "draft", Title = "Draft", Content = "Content", Summary = "Summary",
            Type = PostType.Project, Featured = true, Version = 42,
            CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow
        };
        var service = new AdminPostService(new StubRepository { Post = post });

        var result = await service.GetByIdAsync(post.Id);

        Assert.NotNull(result);
        Assert.Equal(42u, result.Value.Version);
        Assert.Equal(new AdminPostDetailedResponse(post.Id, post.Slug, post.Title, post.Content, post.Summary,
            post.Type, post.Featured, null, post.CreatedAt, post.UpdatedAt), result.Value.Post);
    }

    [Fact]
    public async Task GetByIdReturnsNullWhenMissing()
    {
        Assert.Null(await new AdminPostService(new StubRepository()).GetByIdAsync(Guid.NewGuid()));
    }

    [Theory]
    [InlineData(PostUpdateStatus.Updated)]
    [InlineData(PostUpdateStatus.NotFound)]
    [InlineData(PostUpdateStatus.VersionConflict)]
    [InlineData(PostUpdateStatus.DuplicateSlug)]
    public async Task UpdateMapsEditableFieldsAndPassesVersionAndResult(PostUpdateStatus status)
    {
        var repository = new StubRepository { UpdateStatus = status };
        var id = Guid.NewGuid();
        var publishedAt = new DateTimeOffset(2026, 10, 19, 12, 0, 0, TimeSpan.FromHours(2));
        var request = new UpdatePostRequest("updated-slug", " Updated title ", "Updated content", "",
            PostType.Project, true, publishedAt);

        var result = await new AdminPostService(repository).UpdateAsync(id, request, 42);

        Assert.Equal(status, result);
        Assert.Equal(42u, repository.ExpectedVersion);
        var post = Assert.IsType<Post>(repository.Updated);
        Assert.Equal(id, post.Id);
        Assert.Equal(request.Slug, post.Slug);
        Assert.Equal("Updated title", post.Title);
        Assert.Equal(request.Content, post.Content);
        Assert.Equal("", post.Summary);
        Assert.Equal(PostType.Project, post.Type);
        Assert.True(post.Featured);
        Assert.Equal(publishedAt.UtcDateTime, post.PublishedAt);
    }

    [Fact]
    public async Task UpdateCanResetPublishedPostToDraftAndClearFeaturedFlag()
    {
        var repository = new StubRepository();
        await new AdminPostService(repository).UpdateAsync(Guid.NewGuid(),
            new UpdatePostRequest("draft", "Draft", "Content", "", PostType.Thought, false, null), 42);

        Assert.Null(repository.Updated!.PublishedAt);
        Assert.False(repository.Updated.Featured);
    }

    [Theory]
    [InlineData("slug")]
    [InlineData("title")]
    [InlineData("content")]
    [InlineData("summary")]
    [InlineData("type")]
    [InlineData("featured")]
    [InlineData("publishedAt")]
    public void UpdateRequiresEveryEditableJsonField(string omittedField)
    {
        var fields = new Dictionary<string, object?>
        {
            ["slug"] = "post", ["title"] = "Title", ["content"] = "Content", ["summary"] = "",
            ["type"] = 0, ["featured"] = false, ["publishedAt"] = null
        };
        fields.Remove(omittedField);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<UpdatePostRequest>(
            JsonSerializer.Serialize(fields), options));
    }

    [Theory]
    [InlineData("bad slug", "Title", "Content", "", PostType.Thought)]
    [InlineData("post", " ", "Content", "", PostType.Thought)]
    [InlineData("post", "Title", "", "", PostType.Thought)]
    [InlineData("post", "Title", "Content", null, PostType.Thought)]
    [InlineData("post", "Title", "Content", "", (PostType)99)]
    public void UpdateRejectsInvalidFields(string slug, string title, string content, string? summary, PostType type)
    {
        var request = new UpdatePostRequest(slug, title, content, summary!, type, false, null);
        Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), [], true));
    }

    [Theory]
    [InlineData(0, 0, 1, 1, 0)]
    [InlineData(-1, 500, 1, 100, 0)]
    [InlineData(3, 20, 3, 20, 40)]
    [InlineData(int.MaxValue, 100, int.MaxValue, 100, int.MaxValue)]
    public async Task TitlesNormalizePaginationAndReturnOnlyIdsAndTitles(
        int page, int pageSize, int expectedPage, int expectedSize, int expectedSkip)
    {
        var repository = new StubRepository();
        var result = await new AdminPostService(repository).GetTitlesAsync(new PageQuery(page, pageSize));

        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedSize, result.PageSize);
        Assert.Equal(expectedSkip, repository.Skip);
        Assert.Equal(expectedSize, repository.Take);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(new AdminPostTitleResponse(repository.Id, "Draft title"), Assert.Single(result.Items));
    }

    [Fact]
    public async Task CreateMapsFieldsAndReturnsGeneratedGuid()
    {
        var repository = new StubRepository();
        var publishedAt = new DateTimeOffset(2026, 10, 19, 12, 0, 0, TimeSpan.FromHours(2));
        var request = new CreatePostRequest("new-post", " New post ", "Content", "Summary",
            PostType.Project, true, publishedAt);

        var result = await new AdminPostService(repository).CreateAsync(request);

        Assert.Equal(new AdminPostTitleResponse(repository.Id, "New post"), result);
        var post = Assert.IsType<Post>(repository.Created);
        Assert.Equal(request.Slug, post.Slug);
        Assert.Equal("New post", post.Title);
        Assert.Equal(request.Content, post.Content);
        Assert.Equal(request.Summary, post.Summary);
        Assert.Equal(PostType.Project, post.Type);
        Assert.True(post.Featured);
        Assert.Equal(publishedAt.UtcDateTime, post.PublishedAt);
        Assert.Equal(DateTimeKind.Utc, post.PublishedAt!.Value.Kind);
    }

    [Fact]
    public async Task CreateDefaultsToDraftThought()
    {
        var repository = new StubRepository();
        await new AdminPostService(repository).CreateAsync(new CreatePostRequest("draft", "Draft", "Content"));

        Assert.Null(repository.Created!.PublishedAt);
        Assert.Equal(PostType.Thought, repository.Created.Type);
        Assert.Equal("", repository.Created.Summary);
        Assert.False(repository.Created.Featured);
    }

    [Fact]
    public async Task DuplicateSlugReturnsNull()
    {
        var service = new AdminPostService(new StubRepository { CreateSucceeds = false });
        Assert.Null(await service.CreateAsync(new CreatePostRequest("duplicate", "Title", "Content")));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteUsesGuidAndReturnsWhetherPostExisted(bool exists)
    {
        var repository = new StubRepository { DeleteSucceeds = exists };
        var id = Guid.NewGuid();

        Assert.Equal(exists, await new AdminPostService(repository).DeleteAsync(id));
        Assert.Equal(id, repository.DeletedId);
    }

    [Theory]
    [InlineData("", "Title", "Content", PostType.Thought)]
    [InlineData("not a slug", "Title", "Content", PostType.Thought)]
    [InlineData("valid-slug", " ", "Content", PostType.Thought)]
    [InlineData("valid-slug", "Title", "", PostType.Thought)]
    [InlineData("valid-slug", "Title", "Content", (PostType)99)]
    public void CreateRejectsInvalidFields(string slug, string title, string content, PostType type)
    {
        var request = new CreatePostRequest(slug, title, content, Type: type);
        Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), [], true));
    }

    private sealed class StubRepository : IAdminPostRepository
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public Post? Created { get; private set; }
        public Guid? DeletedId { get; private set; }
        public bool CreateSucceeds { get; init; } = true;
        public bool DeleteSucceeds { get; init; } = true;
        public Post? Post { get; init; }
        public PostUpdateStatus UpdateStatus { get; init; } = PostUpdateStatus.Updated;
        public Post? Updated { get; private set; }
        public uint ExpectedVersion { get; private set; }

        public Task<Post?> GetByIdAsync(Guid id) => Task.FromResult(Post);

        public Task<PostUpdateStatus> UpdateAsync(Post post, uint expectedVersion)
        {
            Updated = post;
            ExpectedVersion = expectedVersion;
            return Task.FromResult(UpdateStatus);
        }

        public Task<(IReadOnlyList<(Guid Id, string Title)> Items, int TotalCount)> GetTitlesAsync(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IReadOnlyList<(Guid Id, string Title)> items = [(Id, "Draft title")];
            return Task.FromResult((items, 5));
        }

        public Task<bool> CreateAsync(Post post)
        {
            Created = post;
            post.Id = Id;
            return Task.FromResult(CreateSucceeds);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            DeletedId = id;
            return Task.FromResult(DeleteSucceeds);
        }
    }
}
