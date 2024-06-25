using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.LikesDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.Hubs;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserService _userService;
        private readonly IFollowerRepository _followerRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<LikeHub> _likeHubContext;

        public LikeService(
            ILikeRepository likeRepository,
            IPostRepository postRepository,
            IUserService userService,
            IFollowerRepository followerRepository,
            IMapper mapper,
            IHubContext<LikeHub> likeHubContext)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _userService = userService;
            _followerRepository = followerRepository;
            _mapper = mapper;
            _likeHubContext = likeHubContext;
        }

        public async Task LikePostAsync(int postId, string userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new GlobalAppException("Post not found.");
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

            await _likeRepository.AddAsync(like);

            // SignalR ile bildirim gönderme
            await _likeHubContext.Clients.All.SendAsync("ReceiveLikeNotification", postId, userId);
        }

        public async Task UnlikePostAsync(int postId, string userId)
        {
            var like = await _likeRepository.GetAsync(l => l.PostId == postId && l.UserId == userId);
            if (like == null)
            {
                throw new GlobalAppException("Like not found.");
            }

            await _likeRepository.DeleteAsync(like);

            // SignalR ile bildirim gönderme
            await _likeHubContext.Clients.All.SendAsync("ReceiveUnlikeNotification", postId, userId);
        }
    }
}
