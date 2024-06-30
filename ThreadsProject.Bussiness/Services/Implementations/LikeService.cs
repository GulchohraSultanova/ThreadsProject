using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.ActionDtos;
using ThreadsProject.Bussiness.DTOs.LikesDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.Hubs;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.RepositoryConcreters;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserService _userService;
        private readonly IFollowerRepository _followerRepository;
        private readonly IUserActionService _userActionService;
        private readonly IMapper _mapper;
        private readonly IHubContext<LikeHub> _likeHubContext;
        private readonly IUserActionRepository _userActionRepository;

        public LikeService(
            ILikeRepository likeRepository,
            IPostRepository postRepository,
            IUserService userService,
            IFollowerRepository followerRepository,
            IMapper mapper,
            IHubContext<LikeHub> likeHubContext,
            IUserActionService userActionService,
            IUserActionRepository userActionRepository)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _userService = userService;
            _followerRepository = followerRepository;
            _mapper = mapper;
            _likeHubContext = likeHubContext;
            _userActionService = userActionService;
            _userActionRepository = userActionRepository;
        }

        public async Task LikePostAsync(int postId, string userId)
        {
            var post = await _postRepository.GetPostWithUserAsync(postId);
            if (post == null)
            {
                throw new GlobalAppException("Post not found.");
            }

            if (post.User == null)
            {
                throw new GlobalAppException("Post owner not found.");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new GlobalAppException("User not found.");
            }

            // Kullanıcı kendi postunu veya takip ettiği postları beğenebilir
            var isFollowing = await _followerRepository.GetAsync(f => f.UserId == post.UserId && f.FollowerUserId == userId);
            if (post.UserId != userId && post.User.IsPublic == false && isFollowing == null)
            {
                throw new GlobalAppException("You can only like posts from users you follow or public profiles.");
            }

            var existingLike = await _likeRepository.GetAsync(l => l.PostId == postId && l.UserId == userId);
            if (existingLike != null)
            {
                throw new GlobalAppException("You have already liked this post.");
            }

            var likeDto = new LikeDto
            {
                PostId = postId,
                UserId = userId
            };

            var like = _mapper.Map<Like>(likeDto);

            try
            {
                await _likeRepository.AddAsync(like);
                // Kullanıcı kendi postunu beğenmediyse
                if (post.UserId != userId)
                {
                    var action = new CreateUserActionDto
                    {
                        UserId = userId,  // Beğeniyi yapan kullanıcı
                        PostId = postId,
                        TargetUserId = post.UserId,
                        ActionType = "PostLiked"
                    };
                    await _userActionService.CreateUserActionAsync(action);
                }
                // SignalR ile bildirim gönderme
                await _likeHubContext.Clients.All.SendAsync("ReceiveLikeNotification", postId, userId);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An unexpected error occurred while liking the post.", ex);
            }
        }






        public async Task UnlikePostAsync(int postId, string userId)
        {
            var like = await _likeRepository.GetAsync(l => l.PostId == postId && l.UserId == userId);
            if (like == null)
            {
                throw new GlobalAppException("Like not found.");
            }

            await _likeRepository.DeleteAsync(like);
            var existingAction = await _userActionRepository.GetAsync(a => a.UserId == userId && a.PostId == postId && a.ActionType == "PostLiked");
            if (existingAction != null)
            {
                await _userActionRepository.DeleteAsync(existingAction);
            }


            // SignalR ile bildirim gönderme
            await _likeHubContext.Clients.All.SendAsync("ReceiveUnlikeNotification", postId, userId);
        }
    }
}
