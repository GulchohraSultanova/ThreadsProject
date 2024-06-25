using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Data.RepositoryConcreters
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly ThreadsContext _context;
        public CommentRepository(ThreadsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Comment> GetCommentWithLikesAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.CommentLikes)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsWithLikesAsync(Expression<Func<Comment, bool>> filter = null)
        {
            IQueryable<Comment> query = _context.Comments.Include(c => c.CommentLikes);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<Comment> GetCommentWithLikesAsync(Expression<Func<Comment, bool>> filter)
        {
            IQueryable<Comment> query = _context.Comments.Include(c => c.CommentLikes);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
