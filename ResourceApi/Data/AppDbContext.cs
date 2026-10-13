using Microsoft.EntityFrameworkCore;
using ResourceApi.Models;

namespace ResourceApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts {get;set;}
    public DbSet<PostTag> PostTags {get;set;}
    public DbSet<Tag> Tags {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("posts");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(p => p.IsProject)
                .HasDefaultValue(false);

            entity.HasIndex(p => p.Slug).IsUnique();

            entity.Property(p => p.Featured)
                .HasDefaultValue(false);

            entity.Property(p => p.Published)
                .HasDefaultValue(false);

            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("now()");

            entity.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(t => t.Name)
                .HasColumnType("citext");

            entity.HasIndex(t => t.Name).IsUnique();
            entity.HasIndex(t => t.Slug).IsUnique();

            entity.Property(t => t.CreatedAt)
                .HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.ToTable("post_tags");

            entity.HasKey(pt => new { pt.PostId, pt.TagId });

            entity.HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
