using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<Post> GetPostWithTagsAndImagesAsync(Expression<Func<Post, bool>> filter)
        {
            return await _context.Posts
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Images) // PostImages'i include edin
                .FirstOrDefaultAsync(filter);
        }

        public async Task<IQueryable<Post>> GetAllPostsWithTagsAndImagesAsync(Expression<Func<Post, bool>>? filter = null)
        {
            IQueryable<Post> query = _context.Posts
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Images); // PostImages'i include edin

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await Task.FromResult(query);
        }
    }
}

