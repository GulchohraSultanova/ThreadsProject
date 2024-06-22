using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.TagDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class TagMapProfilies : Profile
    {
        public  TagMapProfilies()
        {
            CreateMap<Tag, TagGetDto>();
            CreateMap<CreateTagDto, Tag>();
        }
    }
}
