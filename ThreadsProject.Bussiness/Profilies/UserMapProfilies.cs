using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CreateMap<RegisterDto, User>().ReverseMap();
            CreateMap<User, UsersGetDto>()
             .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
             .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
             .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers))
            .ForMember(dest => dest.Followings, opt => opt.MapFrom(src => src.Following));
        }
    }
}


