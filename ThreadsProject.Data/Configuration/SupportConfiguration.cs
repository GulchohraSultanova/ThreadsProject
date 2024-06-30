using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class SupportConfiguration : IEntityTypeConfiguration<Support>
    {
        public void Configure(EntityTypeBuilder<Support> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Description).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.HasOne(s => s.User)
                   .WithMany(u => u.Supports)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        
        }
    }
}
