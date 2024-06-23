
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
          
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserEmailSender, UserEmailSender>();
            services.AddScoped<CustomValidationFilter>();
         
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ITagService, TagService>();

            services.AddScoped<IPostRepository,PostRepository>();
            services.AddScoped<IFollowerRepository,FollowerRepository>();
            services.AddScoped<ITagRepository,TagRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IFollowingRepository,FollowingRepository>();
            services.AddScoped<IFollowService,FollowService>();

         
           
        }
    }
}
