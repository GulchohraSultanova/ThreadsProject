using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.LikesDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public  class LikeMapProfilies:Profile
    {
        public LikeMapProfilies() 
        {
            CreateMap<LikeDto, Like>()
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
