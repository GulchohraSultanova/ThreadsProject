using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.LikesDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public LikeService(ILikeRepository likeRepository, IPostRepository postRepository, IUserService userService, IMapper mapper)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async  Task LikePostAsync(int postId, string userId)
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
        }

        public async Task UnlikePostAsync(int postId, string userId)
        {
            var like = await _likeRepository.GetAsync(l => l.PostId == postId && l.UserId == userId);
            if (like == null)
            {
                throw new GlobalAppException("Like not found.");
            }

            await _likeRepository.DeleteAsync(like);

        }
    }
}
