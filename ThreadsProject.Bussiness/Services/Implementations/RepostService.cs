using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.RepostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class RepostService : IRepostService
    {
        private readonly IRepostRepository _repostRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public RepostService(IRepostRepository repostRepository, IPostRepository postRepository, IMapper mapper)
        {
            _repostRepository = repostRepository;
            _postRepository = postRepository;
            _mapper = mapper;
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
    }
}
