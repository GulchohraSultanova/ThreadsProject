using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.RepostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class RepostService : IRepostService
    {
        private readonly IRepostRepository _repostRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ThreadsContext _context;

        public RepostService(IRepostRepository repostRepository, IPostRepository postRepository, IMapper mapper, UserManager<User> userManager, ThreadsContext context)
        {
            _repostRepository = repostRepository;
            _postRepository = postRepository;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }

        public async Task AddRepostAsync(RepostCreateDto repostCreateDto, string userId)
        {
            var post = await _postRepository.GetByIdAsync(repostCreateDto.PostId);
            if (post == null)
            {
                throw new GlobalAppException("Post not found.");
            }

            var repost = new Repost
            {
                PostId = repostCreateDto.PostId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            try
            {
                await _repostRepository.AddAsync(repost);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while adding the repost.", ex);
            }
        }

        public async Task RemoveRepostAsync(int repostId, string userId)
        {
            var repost = await _repostRepository.GetAsync(r => r.Id == repostId && r.UserId == userId);
            if (repost == null)
            {
                throw new GlobalAppException("Repost not found or user not authorized.");
            }

            try
            {
                await _repostRepository.DeleteAsync(repost);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while removing the repost.", ex);
            }
        }

        public async Task<RepostGetDto> GetRepostByIdAsync(int repostId)
        {
            var repost = await _repostRepository.GetRepostWithDetailsAsync(repostId);
            if (repost == null)
            {
                throw new GlobalAppException("Repost not found.");
            }

            return _mapper.Map<RepostGetDto>(repost);
        }

        public async Task<IEnumerable<RepostGetDto>> GetAllRepostsAsync()
        {
            var reposts = await _repostRepository.GetAllRepostsWithDetailsAsync();
            return _mapper.Map<IEnumerable<RepostGetDto>>(reposts);
        }
        public async Task<IQueryable<RepostGetDto>> GetRepostsByUserIdAsync(string userId, string requesterId)
        {
            var user = await _userManager.Users
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new GlobalAppException("User not found.");
            }

            var isFollowing = await _context.Followers
                .AnyAsync(f => f.UserId == userId && f.FollowerUserId == requesterId);

            if (!user.IsPublic && userId != requesterId && !isFollowing)
            {
                return Enumerable.Empty<RepostGetDto>().AsQueryable();
            }

            var reposts = await _repostRepository.GetRepostsByUserIdWithDetailsAsync(userId);

            return reposts.Select(repost => _mapper.Map<RepostGetDto>(repost)).AsQueryable();
        }

    }
}
