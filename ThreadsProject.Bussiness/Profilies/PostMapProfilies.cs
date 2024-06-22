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
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))
                 .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name).ToList())); ;

            CreateMap<CreatePostDto, Post>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(i => new PostImage { ImageUrl = i }).ToList() : new List<PostImage>()))
                .ForMember(dest => dest.PostTags, opt => opt.Ignore()); 

          
        }
    }
}
