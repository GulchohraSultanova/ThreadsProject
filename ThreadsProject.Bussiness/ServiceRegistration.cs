
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using ThreadsProject.Bussiness.ExternalServices.Implementations;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;

using ThreadsProject.Bussiness.Services.Implementations;
using ThreadsProject.Bussiness.Services.Interfaces;


namespace ThreadsProject.Bussiness
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
          
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserEmailSender, UserEmailSender>();

        }
    }
}
