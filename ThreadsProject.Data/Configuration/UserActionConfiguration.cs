using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class UserActionConfiguration : IEntityTypeConfiguration<UserAction>
    {
        public void Configure(EntityTypeBuilder<UserAction> builder)
        {
            builder.Property(x => x.ActionType).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");

            builder.HasOne(a => a.Post)
                   .WithMany(p => p.Actions)
                   .HasForeignKey(a => a.PostId)
                   .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

        }
    }
}
