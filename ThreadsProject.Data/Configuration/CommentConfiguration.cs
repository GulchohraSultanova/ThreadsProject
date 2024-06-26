using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");

            builder.HasOne(c => c.Post)
                   .WithMany(p => p.Comments)
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Restrict);  // Post silindiğinde, yorumları silme
            builder.HasMany(c => c.CommentLikes)
                 .WithOne(cl => cl.Comment)
                 .HasForeignKey(cl => cl.CommentId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);  // Kullanıcı silindiğinde, yorumları silme
        }
    }
}
