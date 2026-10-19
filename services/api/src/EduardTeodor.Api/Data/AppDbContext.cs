using EduardTeodor.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EduardTeodor.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts {get;set;}
    public DbSet<Tag> Tags {get;set;}

    protected override void OnModelCreating(ModelBuilder build)
    {
        build.Entity<Post>(entity =>
        {
            entity.ToTable("posts");
            entity.Property(p => p.Version).IsRowVersion();
            entity.Property(p => p.Id).HasDefaultValueSql("uuidv7()");
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(p => p.Type).HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<PostType>(v, true));

            entity.HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity(
                    "post_tags",
                    r => r.HasOne(typeof(Tag))
                        .WithMany()
                        .HasForeignKey("tag_id"),
                    l => l.HasOne(typeof(Post))
                        .WithMany()
                        .HasForeignKey("post_id"));
        });

        build.Entity<Tag>(entity =>
        {
           entity.ToTable("tags");
           entity.Property(t => t.Id).HasDefaultValueSql("uuidv7()");
           entity.Property(t => t.CreatedAt).HasDefaultValueSql("now()"); 
        });
    }
}
