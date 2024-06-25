using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Core.RepositoryAbstracts
{
    public interface IRepostRepository : IRepository<Repost>
    {
        Task<Repost> GetRepostWithDetailsAsync(int repostId);
        Task<IEnumerable<Repost>> GetAllRepostsWithDetailsAsync();
        Task<IEnumerable<Repost>> GetRepostsByUserIdWithDetailsAsync(string userId);
    }
}
