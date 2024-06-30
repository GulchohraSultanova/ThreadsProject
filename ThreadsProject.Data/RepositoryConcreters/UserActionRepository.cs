using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Data.RepositoryConcreters
{
    public class UserActionRepository : Repository<UserAction>, IUserActionRepository
    {
        private readonly ThreadsContext _context;
        public UserActionRepository(ThreadsContext context) : base(context)
        {
            _context = context;
        }
        public async Task UpdateAsync(UserAction userAction)
        {
            _context.Set<UserAction>().Update(userAction);
            await _context.SaveChangesAsync();
        }
    }
}
