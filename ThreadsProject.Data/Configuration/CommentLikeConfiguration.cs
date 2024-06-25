using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
    {
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            builder.HasKey(cl => cl.Id);

            builder
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.CommentLikes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Restrict);  // Yorum silindiğinde, CommentLike kayıtlarını silme

            builder
                .HasOne(cl => cl.User)
                .WithMany()
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Kullanıcı silindiğinde, CommentLike kayıtlarını silme
        }
    }
}
