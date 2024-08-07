﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ThreadsProject.Core.Entities;
using ThreadsProject.Data.Configuration;

namespace ThreadsProject.Data.DAL
{
    public class ThreadsContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Repost> Reposts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<UserAction> Actions { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<FollowRequest> Requests { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Support> Supports { get; set; }
        public ThreadsContext(DbContextOptions<ThreadsContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().ToTable("AspNetRoles");
            builder.Entity<User>().ToTable("AspNetUsers");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new PostConfiguration());
            builder.ApplyConfiguration(new RepostConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());
            builder.ApplyConfiguration(new LikeConfiguration());
            builder.ApplyConfiguration(new FollowerConfiguration());
            builder.ApplyConfiguration(new FollowingConfiguration());
            builder.ApplyConfiguration(new UserActionConfiguration());
            builder.ApplyConfiguration(new TagConfiguration());
            builder.ApplyConfiguration(new PostTagConfiguration());
            builder.ApplyConfiguration(new RequestConfiguration());
            builder.ApplyConfiguration(new PostImageConfugration());
            builder.ApplyConfiguration(new CommentLikeConfiguration());
            builder.ApplyConfiguration(new SupportConfiguration());

        }
    }
}
