using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class RepostConfiguration : IEntityTypeConfiguration<Repost>
    {
        public void Configure(EntityTypeBuilder<Repost> builder)
        {
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.HasOne(r => r.Post)
           .WithMany(p => p.Reposts)
           .HasForeignKey(r => r.PostId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Post)
                   .WithMany(p => p.Reposts)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict
        }
    }
}
