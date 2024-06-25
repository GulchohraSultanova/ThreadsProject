using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.FollowsDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class FollowMapProfilies : Profile
    {
        public FollowMapProfilies()
        {
            CreateMap<FollowRequest, FollowRequestDto>()
               .ForMember(dest => dest.SenderUserName, opt => opt.MapFrom(src => src.Sender.UserName))
               .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name)).
                ForMember(dest => dest.SenderSurname, opt => opt.MapFrom(src => src.Sender.Surname)).
                  ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.Sender.IsVerified))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.Sender.ImgUrl));

            CreateMap<FollowRequest, SentFollowRequestDto>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ReceiverUserId, opt => opt.MapFrom(src => src.ReceiverId))

            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));


            CreateMap<Follower, FollowDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FollowerUser.UserName))
                .ForMember(dest=>dest.UserId,opt=>opt.MapFrom(src=>src.FollowerUser.Id))
                .ForMember(dest=>dest.Name,opt=>opt.MapFrom(src=>src.FollowerUser.Name)).
                ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.FollowerUser.Surname)).
                  ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.FollowerUser.IsVerified))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.FollowerUser.ImgUrl)); ; 

            CreateMap<Following, FollowDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FollowingUser.UserName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.FollowingUser.Id)).
                ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FollowingUser.Name)).
                ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.FollowingUser.Surname)).
                  ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.FollowingUser.IsVerified))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.FollowingUser.ImgUrl)); 


        }
    }
}
