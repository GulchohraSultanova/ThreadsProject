using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class RequestConfiguration : IEntityTypeConfiguration<FollowRequest>
    {
        public void Configure(EntityTypeBuilder<FollowRequest> builder)
        {
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.HasOne(fr => fr.Sender)
                   .WithMany(u => u.SentRequests)
                   .HasForeignKey(fr => fr.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fr => fr.Receiver)
                   .WithMany(u => u.ReceivedRequests)
                   .HasForeignKey(fr => fr.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
