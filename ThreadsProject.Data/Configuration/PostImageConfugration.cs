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
    public class PostImageConfugration : IEntityTypeConfiguration<PostImage>
    {
        public void Configure(EntityTypeBuilder<PostImage> builder)
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.ImageUrl).IsRequired();



            builder.HasOne(pi => pi.Post)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("PostImages");
        }
    }
    }

