using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly ITagRepository _tagRepository;

        public PostService(IPostRepository postRepository, IMapper mapper, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _tagRepository = tagRepository;
        }

        public async Task AddPostAsync(CreatePostDto createPostDto, string userId)
        {
            if (createPostDto.Images != null && createPostDto.Images.Count > 4)
            {
                throw new InvalidOperationException("A post can have a maximum of 4 images.");
            }

            var post = _mapper.Map<Post>(createPostDto);
            post.UserId = userId;
            post.CreatedDate = DateTime.UtcNow;

            if (createPostDto.TagIds != null && createPostDto.TagIds.Any())
            {
                foreach (var tagId in createPostDto.TagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(tagId);
                    if (tag != null)
                    {
                        post.PostTags.Add(new PostTag { TagId = tagId });
                    }
                }
            }

            try
            {
                await _postRepository.AddAsync(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while adding the post.", ex);
            }
        }

        public async Task DeletePostAsync(int id, string userId)
        {
            try
            {
                var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.Id == id && p.UserId == userId);
                if (post == null)
                {
                    throw new GlobalAppException("Post not found or user not authorized.");
                }

                await _postRepository.DeleteAsync(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while deleting the post.", ex);
            }
        }

        public async Task<IQueryable<PostGetDto>> GetAllPostsAsync(Expression<Func<Post, bool>>? filter = null, params string[] includes)
        {
            try
            {
                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(filter);
                return posts.Select(post => _mapper.Map<PostGetDto>(post)).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting all posts.", ex);
            }
        }

        public async Task<PostGetDto> GetPostAsync(Expression<Func<Post, bool>>? filter = null, params string[] includes)
        {
            try
            {
                var post = filter != null
                    ? await _postRepository.GetPostWithTagsAndImagesAsync(filter)
                    : await _postRepository.GetPostWithTagsAndImagesAsync(p => true);

                return _mapper.Map<PostGetDto>(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting the post by condition.", ex);
            }
        }
    }
}
