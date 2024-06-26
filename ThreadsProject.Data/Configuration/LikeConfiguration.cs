using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");

            builder.HasOne(l => l.Post)
                   .WithMany(p => p.Likes)
                   .HasForeignKey(l => l.PostId)
                   .OnDelete(DeleteBehavior.Restrict);

   
        }
    }
}
