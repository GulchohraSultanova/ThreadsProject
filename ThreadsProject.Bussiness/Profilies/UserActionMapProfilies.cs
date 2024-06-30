using AutoMapper;
using ThreadsProject.Bussiness.DTOs.ActionDtos;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class UserActionMapProfilies : Profile
    {
        public UserActionMapProfilies()
        {
            // CreateUserActionDto ile UserAction arasındaki eşleme
            CreateMap<CreateUserActionDto, UserAction>();

            // UserAction ile UserActionDto arasındaki eşleme
            CreateMap<UserAction, UserActionDto>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => GenerateActionMessage(src)))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.User.ImgUrl)).
            ForMember(dest => dest.FirstImageUrl, opt => opt.MapFrom(src => src.Post.Images.FirstOrDefault() != null ? src.Post.Images.FirstOrDefault().ImageUrl : null));
        }

        private string GenerateActionMessage(UserAction userAction)
        {
            switch (userAction.ActionType)
            {
                case "PostLiked":
                    return " Liked your post.";
                case "Commented":
                    return "Commented on your post.";
                case "Replied":
                    return "Replied to your comment.";
                case "Reposted":
                    return "Reposted your post.";
                case "Followed":
                    return "Started following you.";
                default:
                    return "Unknown action.";
            }
        }
    }
}
