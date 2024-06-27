using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.CommentDto;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public  interface ICommentService
    {
        Task AddCommentAsync(CreateCommentDto createCommentDto, string userId);
        Task<IEnumerable<CommentGetDto>> GetCommentsByPostIdAsync(int postId);
        Task DeleteCommentAsync(int commentId, string userId);
        Task LikeCommentAsync(int commentId, string userId);
        Task UnlikeCommentAsync(int commentId, string userId);
        Task<IEnumerable<CommentWithPostDto>> GetUserRepliesWithPostsByUserIdAsync(string userId);
        Task<IEnumerable<CommentWithPostDto>> GetMyRepliesWithPostsAsync(string userId);
    }
}
