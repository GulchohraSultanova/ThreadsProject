using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Data.RepositoryConcreters
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly ThreadsContext _context;

        public PostRepository(ThreadsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<Post>> GetAllPostsWithTagsAndImagesAsync(Expression<Func<Post, bool>> filter, params string[] includes)
        {
            IQueryable<Post> query = _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Comments).ThenInclude(c => c.CommentLikes);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await Task.FromResult(query);
        }

        public async Task<Post> GetPostWithTagsAndImagesAsync(Expression<Func<Post, bool>> filter, params string[] includes)
        {
            IQueryable<Post> query = _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Comments).ThenInclude(c => c.CommentLikes);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
