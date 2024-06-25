using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly ITagRepository _tagRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly UserManager<User> _userManager;
        private readonly ThreadsContext _context;

        public PostService(
            IPostRepository postRepository,
            IMapper mapper,
            ITagRepository tagRepository,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository,
            IFollowerRepository followerRepository,
            UserManager<User> userManager,
            ThreadsContext context)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _followerRepository = followerRepository;
            _userManager = userManager;
            _context = context;
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
                var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.Id == id && p.UserId == userId, "Comments", "Comments.CommentLikes", "Likes", "User");
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

        public async Task<IQueryable<PostGetDto>> GetExplorePostsAsync()
        {
            try
            {
                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(p => p.User.IsPublic && !p.User.IsDeleted, "Comments", "Comments.CommentLikes", "Likes", "User");
                return posts.Select(post => _mapper.Map<PostGetDto>(post)).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting explore posts.", ex);
            }
        }

        public async Task<IQueryable<PostGetDto>> GetHomePostsAsync(string userId)
        {
            try
            {
                var followings = await _followerRepository.GetAllAsync(f => f.FollowerUserId == userId);
                var followingIds = followings.Select(f => f.UserId).ToList();

                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(p => followingIds.Contains(p.UserId) && !p.User.IsDeleted, "Comments", "Comments.CommentLikes", "Likes", "User");
                return posts.Select(post => _mapper.Map<PostGetDto>(post)).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting home posts.", ex);
            }
        }

        public async Task<PostGetDto> GetPostAsync(Expression<Func<Post, bool>>? filter = null, params string[] includes)
        {
            try
            {
                var post = filter != null
                    ? await _postRepository.GetPostWithTagsAndImagesAsync(filter, includes)
                    : await _postRepository.GetPostWithTagsAndImagesAsync(p => true, includes);

                return _mapper.Map<PostGetDto>(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting the post by condition.", ex);
            }
        }

        public async Task<IQueryable<PostGetDto>> GetUserPostsAsync(string userId)
        {
            try
            {
                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(p => p.UserId == userId && !p.User.IsDeleted, "Comments", "Comments.CommentLikes", "Likes", "User");
                return posts.Select(post => _mapper.Map<PostGetDto>(post)).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting the user posts.", ex);
            }
        }

        public async Task<IQueryable<PostGetDto>> GetPostsByUserIdAsync(string userId, string requesterId)
        {
            try
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
                    return Enumerable.Empty<PostGetDto>().AsQueryable();
                }

                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(
                    p => p.UserId == userId && !p.User.IsDeleted,
                    "Comments", "Comments.CommentLikes", "Likes", "User"
                );

                return posts.Select(post => _mapper.Map<PostGetDto>(post)).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while retrieving the user's posts.", ex);
            }
        }
    }
}
