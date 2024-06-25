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
    public class CommentLikeRepository : Repository<CommentLike>, ICommentLikeRepository
    {
        private readonly ThreadsContext _context;

        public CommentLikeRepository(ThreadsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CommentLike> GetCommentLikeAsync(int commentId, string userId)
        {
            return await _context.CommentLikes
                .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);
        }
    }
}
