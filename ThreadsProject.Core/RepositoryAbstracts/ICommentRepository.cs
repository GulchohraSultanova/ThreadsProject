using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Core.RepositoryAbstracts
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<Comment> GetCommentWithLikesAsync(int commentId);
        Task<IEnumerable<Comment>> GetAllCommentsWithLikesAsync(Expression<Func<Comment, bool>> filter = null);
        Task<Comment> GetCommentWithLikesAsync(Expression<Func<Comment, bool>> filter);
    }
}
