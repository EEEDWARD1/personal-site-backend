using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PostTag> PostTags => Set<PostTag>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("citext");
        b.HasPostgresEnum<PostKind>("post_kind");

        b.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasIndex(x => x.KeycloakUserId).IsUnique();
            e.Property(x => x.Username).HasMaxLength(50);
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
        });

        b.Entity<Post>(e =>
        {
            e.ToTable("posts");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => x.Kind).HasDatabaseName("idx_posts_kind");
            e.HasIndex(x => x.Published).HasDatabaseName("idx_posts_published");
        });

        b.Entity<Tag>(e =>
        {
            e.ToTable("tags");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(x => x.Name).HasColumnType("citext");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.HasIndex(x => x.Name).IsUnique();
            e.HasIndex(x => x.Slug).IsUnique();
        });

        b.Entity<PostTag>(e =>
        {
            e.ToTable("post_tags");
            e.HasKey(x => new { x.PostId, x.TagId });   // composite PK
            e.HasOne(x => x.Post).WithMany(p => p.PostTags)
                .HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Tag).WithMany(t => t.PostTags)
                .HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.TagId).HasDatabaseName("idx_post_tags_tag_id");
        });
    }
}