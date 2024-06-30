using AutoMapper;
using ThreadsProject.Bussiness.DTOs.CommentDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class CommentMapProfilies : Profile
    {
        public CommentMapProfilies()
        {
            CreateMap<CreateCommentDto, Comment>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId));

            CreateMap<Comment, CommentGetDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest=> dest.UserName, opt=>opt.MapFrom(src=>src.User.UserName))
                .ForMember(dest => dest.IsVerifed, opt => opt.MapFrom(src => src.User.IsVerified))

                .ForMember(dest=>dest.ImgUrl, opt=>opt.MapFrom(src=>src.User.ImgUrl))
                .ForMember(dest=>dest.PostId,opt=>opt.MapFrom(src=>src.PostId))
                 .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
                  .ForMember(dest => dest.ReplyUsername, opt => opt.MapFrom(src => src.ReplyUsername))
                .ForMember(dest => dest.LikeUserIds, opt => opt.MapFrom(src => src.CommentLikes.Select(cl => cl.UserId).ToList()));
        }
    }
}
