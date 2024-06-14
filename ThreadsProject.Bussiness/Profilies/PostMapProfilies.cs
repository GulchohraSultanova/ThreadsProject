using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class PostMapProfilies : Profile
    {
        public  PostMapProfilies()
        {
            CreateMap<Post, PostGetDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()));

            CreateMap<CreatePostDto, Post>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(i => new PostImage { ImageUrl = i }).ToList() : new List<PostImage>()));

            CreateMap<UpdatePostDto, Post>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(i => new PostImage { ImageUrl = i }).ToList() : new List<PostImage>()));
        }
    }
}
