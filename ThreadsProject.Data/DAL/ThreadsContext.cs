using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;
using ThreadsProject.Data.Configuration;

namespace ThreadsProject.Data.DAL
{
    public class ThreadsContext : IdentityDbContext<User>
    {
        public ThreadsContext(DbContextOptions<ThreadsContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
         
        }
    }
}
