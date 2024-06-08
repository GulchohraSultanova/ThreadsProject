using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.Services.Implementations;
using ThreadsProject.Bussiness.Services.Interfaces;

namespace ThreadsProject.Bussiness
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService,UserService>();
        }
    }
}
