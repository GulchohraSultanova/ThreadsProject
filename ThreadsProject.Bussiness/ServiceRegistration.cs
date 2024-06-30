
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThreadsProject.Bussiness.ExternalServices.Implementations;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;

using ThreadsProject.Bussiness.Services.Implementations;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.RepositoryConcreters;
using YourApiProject.Filters;


namespace ThreadsProject.Bussiness
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
          
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserEmailSender, UserEmailSender>();
            services.AddScoped<CustomValidationFilter>();
         
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IRepostService, RepostService>();
            services.AddScoped<ISupportService, SupportService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserActionService, UserActionService>();
  



            services.AddScoped<IPostRepository,PostRepository>();
            services.AddScoped<IFollowerRepository,FollowerRepository>();
            services.AddScoped<ITagRepository,TagRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IFollowingRepository,FollowingRepository>();
            services.AddScoped<ILikeRepository,LikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
            services.AddScoped<IRepostRepository, RepostRepository>();
            services.AddScoped<ISupportRepository, SupportRepository>();
            services.AddScoped<IUserActionRepository, UserActionRepository>();
            services.AddSignalR();
            var cloudinarySettings = configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var cloudinaryAccount = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            services.AddSingleton(cloudinary);



        }
    }
}
