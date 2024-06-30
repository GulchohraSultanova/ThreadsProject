using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public interface IPostService
    {
    
        Task<IQueryable<PostGetDto>> GetHomePostsAsync(string userId);
        Task<PostGetDto> GetPostAsync(Expression<Func<Post, bool>>? filter = null, params string[] includes);
        Task AddPostAsync(CreatePostDto createPostDto, string userId);
        Task<IQueryable<PostGetDto>> GetUserPostsAsync(string userId);
        Task DeletePostAsync(int id, string userId);
        Task<IQueryable<PostGetDto>> GetPostsByUserIdAsync(string userId, string requesterId);
        Task<PostGetDto> GetPostByUserIdAndPostIdAsync(string userId, int postId);
        Task<IEnumerable<PostGetDto>> GetExplorePostsAsync(string userId, int countPerTag);
        Task<IEnumerable<PostGetDto>> GetLikedPostsByUserAsync(string userId);
        Task<IEnumerable<PostGetDto>> GetRandomPublicPostsAsync(int count, HashSet<int> seenPostIds);
    }
}
