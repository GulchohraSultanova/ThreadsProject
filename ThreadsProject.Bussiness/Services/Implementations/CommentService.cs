using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.ActionDtos;
using ThreadsProject.Bussiness.DTOs.CommentDto;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.RepositoryConcreters;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly IMapper _mapper;
        private readonly IUserActionService _userActionService;
        private readonly IUserActionRepository _userActionRepository;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IFollowerRepository followerRepository, IMapper mapper, ICommentLikeRepository commentLikeRepository, IUserActionService userActionService, IUserActionRepository userActionRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _followerRepository = followerRepository;
            _mapper = mapper;
            _commentLikeRepository = commentLikeRepository;
            _userActionService = userActionService;
            _userActionRepository = userActionRepository;
        }

        public async Task AddCommentAsync(CreateCommentDto createCommentDto, string userId)
        {
            var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.Id == createCommentDto.PostId, "Comments");
            if (post == null)
            {
                throw new GlobalAppException("Post not found.");
            }

            var canComment = post.UserId == userId || post.User.IsPublic || await _followerRepository.GetAsync(f => f.UserId == post.UserId && f.FollowerUserId == userId) != null;
            if (!canComment)
            {
                throw new GlobalAppException("You are not allowed to comment on this post.");
            }

            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.UserId = userId;
            comment.CreatedDate = DateTime.UtcNow;

            try
            {
                await _commentRepository.AddAsync(comment);

                // Kullanıcı kendi postuna yorum yapmıyorsa aksiyon ekleyelim
                if (post.UserId != userId)
                {
                    var action = new CreateUserActionDto
                    {
                        UserId = userId,  // Yorumu yapan kullanıcı
                        PostId = post.Id,
                        TargetUserId = post.UserId,
                        CommentId = comment.Id,
                        ActionType = "Commented"
                    };
                    await _userActionService.CreateUserActionAsync(action);
                }
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while adding the comment.", ex);
            }
        }



        public async Task<IEnumerable<CommentGetDto>> GetCommentsByPostIdAsync(int postId)
        {
            var comments = await _commentRepository.GetAllCommentsWithLikesAsync(c => c.PostId == postId && c.ParentCommentId == null); // Fetch only top-level comments
            var commentDtos = _mapper.Map<IEnumerable<CommentGetDto>>(comments);

            foreach (var commentDto in commentDtos)
            {
                await LoadRepliesAsync(commentDto);
            }

            return commentDtos;
        }

        private async Task LoadRepliesAsync(CommentGetDto commentDto)
        {
            var replies = await _commentRepository.GetAllCommentsWithLikesAsync(c => c.ParentCommentId == commentDto.Id);
            var replyDtos = _mapper.Map<IEnumerable<CommentGetDto>>(replies);

            commentDto.Replies = replyDtos.ToList();

            foreach (var replyDto in commentDto.Replies)
            {
                await LoadRepliesAsync(replyDto); // Recursively load replies
            }
        }
        public async Task DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _commentRepository.GetAsync(c => c.Id == commentId && c.UserId == userId, "Replies");
            if (comment == null)
            {
                throw new GlobalAppException("Comment not found or user not authorized.");
            }

            try
            {
                // Silinecek yorumun aksiyonlarını da sil
                var actions = await _userActionRepository.GetAllAsync(a => a.CommentId == commentId);
                foreach (var action in actions)
                {
                    await _userActionRepository.DeleteAsync(action);
                }

                // Tüm yanıtları sil
                foreach (var reply in comment.Replies.ToList())
                {
                    await DeleteCommentAndRepliesAsync(reply.Id, userId);
                }

                // Yorumu sil
                await _commentRepository.DeleteAsync(comment);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while deleting the comment.", ex);
            }
        }

        private async Task DeleteCommentAndRepliesAsync(int commentId, string userId)
        {
            var comment = await _commentRepository.GetAsync(c => c.Id == commentId, "Replies");
            if (comment != null)
            {
                // Silinecek yorumun aksiyonlarını da sil
                var actions = await _userActionRepository.GetAllAsync(a => a.CommentId == commentId);
                foreach (var action in actions)
                {
                    await _userActionRepository.DeleteAsync(action);
                }

                foreach (var reply in comment.Replies.ToList())
                {
                    await DeleteCommentAndRepliesAsync(reply.Id, userId);
                }
                await _commentRepository.DeleteAsync(comment);
            }
        }



        public async Task LikeCommentAsync(int commentId, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new GlobalAppException("Comment not found.");
            }

            var existingLike = await _commentLikeRepository.GetCommentLikeAsync(commentId, userId);
            if (existingLike != null)
            {
                throw new GlobalAppException("You have already liked this comment.");
            }

            var commentLike = new CommentLike
            {
                CommentId = commentId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            try
            {
                await _commentLikeRepository.AddAsync(commentLike);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while liking the comment.", ex);
            }
        }

        public async Task UnlikeCommentAsync(int commentId, string userId)
        {
            var commentLike = await _commentLikeRepository.GetCommentLikeAsync(commentId, userId);
            if (commentLike == null)
            {
                throw new GlobalAppException("Like not found.");
            }

            try
            {
                await _commentLikeRepository.DeleteAsync(commentLike);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while unliking the comment.", ex);
            }
        }

        public async Task<IEnumerable<CommentWithPostDto>> GetUserRepliesWithPostsByUserIdAsync(string userId)
        {
            var comments = await _commentRepository.GetAllAsync(c => c.UserId == userId);

            var result = new List<CommentWithPostDto>();

            foreach (var comment in comments)
            {
                var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.Id == comment.PostId, "Comments", "Comments.CommentLikes", "Likes", "User");
                if (post != null)
                {
                    var commentWithPostDto = new CommentWithPostDto
                    {
                        CommentId = comment.Id,
                        CommentContent = comment.Content,
                        CommentCreatedDate = comment.CreatedDate,
                        Post = _mapper.Map<PostGetDto>(post)
                    };
                    result.Add(commentWithPostDto);
                }
            }

            return result;
        }

        public async Task<IEnumerable<CommentWithPostDto>> GetMyRepliesWithPostsAsync(string userId)
        {
            var comments = await _commentRepository.GetAllAsync(c => c.UserId == userId);

            var result = new List<CommentWithPostDto>();

            foreach (var comment in comments)
            {
                var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.Id == comment.PostId, "Comments", "Comments.CommentLikes", "Likes", "User");
                if (post != null)
                {
                    var commentWithPostDto = new CommentWithPostDto
                    {
                        CommentId = comment.Id,
                        CommentContent = comment.Content,
                        CommentCreatedDate = comment.CreatedDate,
                        Post = _mapper.Map<PostGetDto>(post)
                    };
                    result.Add(commentWithPostDto);
                }
            }

            return result;
        }
    }
}
