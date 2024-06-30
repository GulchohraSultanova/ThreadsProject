using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Core.RepositoryAbstracts
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IQueryable<Post>> GetAllPostsWithTagsAndImagesAsync(Expression<Func<Post, bool>> filter, params string[] includes);
        Task<Post> GetPostWithTagsAndImagesAsync(Expression<Func<Post, bool>> filter, params string[] includes);
        Task<Post> GetPostWithUserAsync(int postId);
        Task<IEnumerable<Post>> GetRandomPostsByTagsAsync(string userId, int postsPerTag = 5);
     
    }
}
