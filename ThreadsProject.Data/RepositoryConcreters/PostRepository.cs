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
        public async Task<Post> GetPostWithUserAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<IEnumerable<Post>> GetRandomPostsByTagsAsync(string userId, int postsPerTag = 5)
        {
            // Kullanıcının kendi postlarını ve takip ettiği kullanıcıların postlarını hariç tut
            var followingUserIds = await _context.Followers
                .Where(f => f.FollowerUserId == userId)
                .Select(f => f.UserId)
                .ToListAsync();

            var excludedUserIds = followingUserIds.Append(userId).ToList();

            var tags = await _context.Tags.ToListAsync();
            var randomPosts = new List<Post>();
            var seenPostIds = new HashSet<int>();
            var random = new Random();

            foreach (var tag in tags)
            {
                var postsForTag = await _context.Posts
                    .Include(p => p.PostTags)
                    .Include(p => p.Likes)
                    .Where(p => p.PostTags.Any(pt => pt.TagId == tag.Id) && !excludedUserIds.Contains(p.UserId))
                    .ToListAsync();

                var uniquePosts = postsForTag
                    .Where(p => !seenPostIds.Contains(p.Id)) // Görülen postları hariç tut
                    .OrderBy(p => random.Next())
                    .Take(postsPerTag)
                    .ToList();

                foreach (var post in uniquePosts)
                {
                    seenPostIds.Add(post.Id);
                }

                randomPosts.AddRange(uniquePosts);
            }

            return randomPosts;
        }



    }
}
