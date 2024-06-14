using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }


        public async Task AddPostAsync(CreatePostDto createPostDto, string userId)
        {
            if (createPostDto.Images != null && createPostDto.Images.Count > 4)
            {
                throw new InvalidOperationException("A post can have a maximum of 4 images.");
            }
            try
            {
               
                var post = _mapper.Map<Post>(createPostDto);
                post.UserId = userId;
                post.CreatedDate = DateTime.UtcNow;

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
                var post = await _postRepository.GetAsync(p => p.Id == id && p.UserId == userId);
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
                var posts = await _postRepository.GetAllAsync(filter, includes);
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
                    ? await _postRepository.GetAsync(filter, includes)
                    : await _postRepository.GetAsync(p => true, includes); // Default filter to return first item

                return _mapper.Map<PostGetDto>(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting the post by condition.", ex);
            }
        }

       
    }
}
