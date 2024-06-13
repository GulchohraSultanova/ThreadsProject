using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> builder)
        {
            builder.HasKey(pt => new { pt.PostId, pt.TagId });

            builder.HasOne(pt => pt.Post)
                   .WithMany(p => p.PostTags)
                   .HasForeignKey(pt => pt.PostId);

            builder.HasOne(pt => pt.Tag)
                   .WithMany(t => t.PostTags)
                   .HasForeignKey(pt => pt.TagId);
        }
    }
}
