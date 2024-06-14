using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(x => x.Content).IsRequired().HasMaxLength(500);
          
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.HasMany(p => p.Images)
           .WithOne(i => i.Post)
           .HasForeignKey(i => i.PostId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Comments)
                   .WithOne(c => c.Post)
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(p => p.Likes)
                   .WithOne(l => l.Post)
                   .HasForeignKey(l => l.PostId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(p => p.Reposts)
                   .WithOne(r => r.Post)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(p => p.Actions)
                   .WithOne(a => a.Post)
                   .HasForeignKey(a => a.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
