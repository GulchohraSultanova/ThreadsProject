using AutoMapper;
using ThreadsProject.Bussiness.DTOs.CommentDto;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class PostMapProfilies : Profile
    {
        public PostMapProfilies()
        {
            CreateMap<Post, PostGetDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name).ToList()))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Select(l => l.UserId).ToList()))

                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Select(c => new CommentGetDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    UserId = c.UserId,
                    PostId = c.PostId, // PostId alanını ekliyoruz
                    LikeUserIds = c.CommentLikes.Select(cl => cl.UserId).ToList()
                }).ToList()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.User.IsVerified))

                .ForMember(dest => dest.UserImgUrl, opt => opt.MapFrom(src => src.User.ImgUrl));

            CreateMap<CreatePostDto, Post>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(i => new PostImage { ImageUrl = i }).ToList() : new List<PostImage>()))
                .ForMember(dest => dest.PostTags, opt => opt.Ignore());
        }
    }
}
