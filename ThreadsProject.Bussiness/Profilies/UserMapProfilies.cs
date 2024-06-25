using AutoMapper;
using ThreadsProject.Bussiness.DTOs.CommentDto;
using ThreadsProject.Bussiness.DTOs.FollowsDto;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.DTOs.RepostDto;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class UserMapProfilies : Profile
    {
        public UserMapProfilies()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

            CreateMap<User, UsersGetDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers.Select(f => new FollowDto
                {
                    UserId = f.FollowerUserId,
                    Name = f.FollowerUser.Name,
                    Surname = f.FollowerUser.Surname,
                    UserName = f.FollowerUser.UserName,
                    ImgUrl = f.FollowerUser.ImgUrl,
                    IsVerified = f.FollowerUser.IsVerified

                }).ToList()))
                .ForMember(dest => dest.Followings, opt => opt.MapFrom(src => src.Following.Select(f => new FollowDto
                {
                    UserId = f.FollowingUserId,
                    Name = f.FollowingUser.Name,
                    Surname = f.FollowingUser.Surname,
                    UserName = f.FollowingUser.UserName,
                    ImgUrl = f.FollowingUser.ImgUrl,
                    IsVerified = f.FollowingUser.IsVerified
                }).ToList()));
           
         
        }
    }
}
