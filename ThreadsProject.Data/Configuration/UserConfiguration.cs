using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Surname).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(512);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(256);

            builder.HasIndex(x=> x.Email).IsUnique();
            builder.HasIndex(x => x.UserName).IsUnique();

            builder.HasMany(u => u.Posts)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId);

            builder.HasMany(u => u.Reposts)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.UserId);

            builder.HasMany(u => u.Actions)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId);

            builder.HasMany(u => u.Followers)
                   .WithOne(f => f.User)
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Following)
                   .WithOne(f => f.User)
                   .HasForeignKey(f => f.FollowingUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.SentRequests)
                   .WithOne(r => r.Sender)
                   .HasForeignKey(r => r.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ReceivedRequests)
                   .WithOne(r => r.Receiver)
                   .HasForeignKey(r => r.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
