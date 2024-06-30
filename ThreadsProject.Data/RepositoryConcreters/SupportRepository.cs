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
    public class SupportRepository : Repository<Support>, ISupportRepository
    {
        public SupportRepository(ThreadsContext context) : base(context)
        {
        }
    }
}
