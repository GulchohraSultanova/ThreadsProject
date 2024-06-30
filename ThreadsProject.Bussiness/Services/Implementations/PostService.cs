using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.Exceptions;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;
using static System.Net.Mime.MediaTypeNames;

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
        private readonly Cloudinary _cloudinary;

        public PostService(
            IPostRepository postRepository,
            IMapper mapper,
            ITagRepository tagRepository,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository,
            IFollowerRepository followerRepository,
            UserManager<User> userManager,
               Cloudinary cloudinary,
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
            _cloudinary = cloudinary;
        }
        private async Task<string> UploadMediaToCloudinaryAsync(IFormFile mediaFile)
        {
            RawUploadParams uploadParams;

            if (mediaFile.ContentType.StartsWith("video/"))
            {
                uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(mediaFile.FileName, mediaFile.OpenReadStream()),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
            }
            else
            {
                uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(mediaFile.FileName, mediaFile.OpenReadStream()),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
            }

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task AddPostAsync(CreatePostDto createPostDto, string userId)
        {
            if (createPostDto.Images != null && createPostDto.Images.Count > 4)
            {
                throw new InvalidOperationException("A post can have a maximum of 4 media files.");
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

            if (createPostDto.Images != null && createPostDto.Images.Any())
            {
                post.Images = new List<PostImage>();
                foreach (var base64Image in createPostDto.Images)
                {
                    IFormFile file = ConvertBase64ToIFormFile(base64Image); // Base64 string'den IFormFile'e dönüştürme
                    var uploadUrl = await UploadMediaToCloudinaryAsync(file);
                    post.Images.Add(new PostImage
                    {
                        ImageUrl = uploadUrl
                    });
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

        private IFormFile ConvertBase64ToIFormFile(string base64String)
        {
            var base64Parts = base64String.Split(",");
            var data = base64Parts.Length > 1 ? base64Parts[1] : base64Parts[0];
            byte[] bytes = Convert.FromBase64String(data);
            var stream = new MemoryStream(bytes);
            var fileName = $"upload_{Guid.NewGuid()}";
            var contentType = GetContentTypeFromBase64(base64Parts[0]);

            return new FormFile(stream, 0, stream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        private string GetContentTypeFromBase64(string base64Header)
        {
            if (base64Header.Contains("data:image/jpeg")) return "image/jpeg";
            if (base64Header.Contains("data:image/png")) return "image/png";
            if (base64Header.Contains("data:image/gif")) return "image/gif";
            if (base64Header.Contains("data:image/bmp")) return "image/bmp";
            if (base64Header.Contains("data:image/webp")) return "image/webp";
            if (base64Header.Contains("data:image/heic")) return "image/heic";
            if (base64Header.Contains("data:image/tiff") || base64Header.Contains("data:image/tif")) return "image/tiff";
            if (base64Header.Contains("data:image/raw") || base64Header.Contains("data:image/dng") || base64Header.Contains("data:image/cr2") || base64Header.Contains("data:image/nef") || base64Header.Contains("data:image/arw")) return "image/raw";
            if (base64Header.Contains("data:image/heif")) return "image/heif";
            if (base64Header.Contains("data:image/hevc")) return "image/hevc";
            if (base64Header.Contains("data:video/mp4")) return "video/mp4";
            if (base64Header.Contains("data:video/quicktime")) return "video/quicktime";
            if (base64Header.Contains("data:video/3gpp")) return "video/3gpp";
            if (base64Header.Contains("data:video/x-matroska")) return "video/x-matroska";
            if (base64Header.Contains("data:video/x-msvideo")) return "video/x-msvideo";
            if (base64Header.Contains("data:video/x-flv")) return "video/x-flv";
            if (base64Header.Contains("data:video/x-ms-wmv")) return "video/x-ms-wmv";
            if (base64Header.Contains("data:video/webm")) return "video/webm";

            throw new GlobalAppException("Unsupported file format.");
        }




        public async Task DeletePostAsync(int id, string userId)
        {
            try
            {
                var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.Id == id && p.UserId == userId, "Comments", "Comments.CommentLikes", "Likes", "User", "Reposts", "Actions");
                if (post == null)
                {
                    throw new GlobalAppException("Post not found or user not authorized.");
                }

                // Delete all related data
                _context.Comments.RemoveRange(post.Comments);
                _context.Likes.RemoveRange(post.Likes);
                _context.Reposts.RemoveRange(post.Reposts);
                _context.Actions.RemoveRange(post.Actions);

                await _context.SaveChangesAsync();

                // Finally delete the post
                await _postRepository.DeleteAsync(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while deleting the post.", ex);
            }
        }

        public async Task<IEnumerable<PostGetDto>> GetExplorePostsAsync(string userId, int countPerTag)
        {
            var tags = await _tagRepository.GetAllAsync();
            var userFollowingIds = await _followerRepository.GetAllAsync(f => f.FollowerUserId == userId);
            var followingUserIds = userFollowingIds.Select(f => f.UserId).ToList();

            var posts = new List<Post>();
            var seenPostIds = new HashSet<int>();

            foreach (var tag in tags)
            {
                var tagPosts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(
                    p => p.PostTags.Any(pt => pt.TagId == tag.Id) &&
                         p.UserId != userId &&
                         !followingUserIds.Contains(p.UserId) &&
                         p.User.IsPublic && !p.User.IsBanned && ! p.User.IsDeleted,
                    "Comments", "Comments.CommentLikes", "Likes", "User");

                var uniquePosts = tagPosts
                    .Where(p => !seenPostIds.Contains(p.Id))
                    .OrderBy(x => Guid.NewGuid())
                    .Take(countPerTag)
                    .ToList();

                foreach (var post in uniquePosts)
                {
                    seenPostIds.Add(post.Id);
                }

                posts.AddRange(uniquePosts);
            }

            return posts.Select(post => _mapper.Map<PostGetDto>(post));
        }
        public async Task<IEnumerable<PostGetDto>> GetLikedPostsByUserAsync(string userId)
        {
            var likedPosts = await _likeRepository.GetAllAsync(like => like.UserId == userId);

            var postIds = likedPosts.Select(like => like.PostId).ToList();

            var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(
                p => postIds.Contains(p.Id) && p.User.IsPublic && !p.User.IsBanned && !p.User.IsDeleted,
                "Comments", "Comments.CommentLikes", "Likes", "User");

            return posts.Select(post => _mapper.Map<PostGetDto>(post));
        }

        public async Task<IQueryable<PostGetDto>> GetHomePostsAsync(string userId)
        {
            try
            {
                var followings = await _followerRepository.GetAllAsync(f => f.FollowerUserId == userId);
                var followingIds = followings.Select(f => f.UserId).ToList();

                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(p => followingIds.Contains(p.UserId) && !p.User.IsDeleted && !p.User.IsBanned, "Comments", "Comments.CommentLikes", "Likes", "User");
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
                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(p => p.UserId == userId && !p.User.IsDeleted && !p.User.IsBanned, "Comments", "Comments.CommentLikes", "Likes", "User");
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
                    throw new PrivateProfileException("This profile is private!");
                }

                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(
                    p => p.UserId == userId && !p.User.IsDeleted,
                    "Comments", "Comments.CommentLikes", "Likes", "User"
                );

                return posts.Select(post => _mapper.Map<PostGetDto>(post)).AsQueryable();
            }
            catch (Exception ex) when (ex is not PrivateProfileException)
            {
                throw new GlobalAppException("An error occurred while retrieving the user's posts.", ex);
            }
        }
        public async Task<IEnumerable<PostGetDto>> GetRandomPublicPostsAsync(int count, HashSet<int> seenPostIds)
        {
            var random = new Random();

            // Tüm gerekli postları veritabanından alıyoruz.
            var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(
                p => p.User.IsPublic && !p.User.IsBanned && ! p.User.IsDeleted && !seenPostIds.Contains(p.Id),
                "Comments", "Comments.CommentLikes", "Likes", "User");

            // Bellek içi sıralama ve belirli bir sayıda post alma
            var randomPosts = posts
                .AsEnumerable() // Veritabanı sorgusunu burada tamamlayın ve bellek içi işlemleri başlatın
                .OrderBy(x => random.Next())
                .Take(count)
                .ToList();

            foreach (var post in randomPosts)
            {
                seenPostIds.Add(post.Id);
            }

            return randomPosts.Select(post => _mapper.Map<PostGetDto>(post));
        }




        public async Task<IEnumerable<PostGetDto>> GetPostsByTagAsync(int tagId)
        {
            try
            {
                var posts = await _postRepository.GetAllPostsWithTagsAndImagesAsync(
                    p => p.PostTags.Any(pt => pt.TagId == tagId) && p.User.IsPublic && !p.User.IsBanned && !p.User.IsDeleted,
                    "Comments", "Comments.CommentLikes", "Likes", "User"
                );

                var sortedPosts = posts.OrderByDescending(p => p.Likes.Count).ToList();

                return sortedPosts.Select(post => _mapper.Map<PostGetDto>(post));
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while retrieving the posts by tag.", ex);
            }
        }



        public async Task<PostGetDto> GetPostByUserIdAndPostIdAsync(string userId, int postId)
        {
            try
            {
                var post = await _postRepository.GetPostWithTagsAndImagesAsync(p => p.UserId == userId && p.Id == postId, "Comments", "Comments.CommentLikes", "Likes", "User");
                if (post == null)
                {
                    throw new GlobalAppException("Post not found.");
                }

                return _mapper.Map<PostGetDto>(post);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while retrieving the user's post.", ex);
            }
        }
    }
}
