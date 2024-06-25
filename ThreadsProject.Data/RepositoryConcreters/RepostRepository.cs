using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Data.RepositoryConcreters
{
    public class RepostRepository : Repository<Repost>, IRepostRepository
    {
        private readonly ThreadsContext _context;

        public RepostRepository(ThreadsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Repost> GetRepostWithDetailsAsync(int repostId)
        {
            return await _context.Reposts
                .Include(r => r.Post)
                    .ThenInclude(p => p.User)
                .Include(r => r.Post)
                    .ThenInclude(p => p.Images)
                .Include(r => r.Post)
                    .ThenInclude(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                .Include(r => r.Post)
                    .ThenInclude(p => p.Likes)
                .Include(r => r.Post)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.CommentLikes)
                .FirstOrDefaultAsync(r => r.Id == repostId);
        }

        public async Task<IEnumerable<Repost>> GetAllRepostsWithDetailsAsync()
        {
            return await _context.Reposts
                .Include(r => r.Post)
                    .ThenInclude(p => p.User)
                .Include(r => r.Post)
                    .ThenInclude(p => p.Images)
                .Include(r => r.Post)
                    .ThenInclude(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                .Include(r => r.Post)
                    .ThenInclude(p => p.Likes)
                .Include(r => r.Post)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.CommentLikes)
                .ToListAsync();
        }
    }
}
