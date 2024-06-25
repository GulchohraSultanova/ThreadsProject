using AutoMapper;
using ThreadsProject.Bussiness.DTOs.RepostDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class RepostMapProfilies : Profile
    {
        public RepostMapProfilies()
        {
            CreateMap<Repost, RepostGetDto>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Post));
            CreateMap<RepostCreateDto, Repost>();
        }
    }
}
