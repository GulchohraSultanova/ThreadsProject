using AutoMapper;

using ThreadsProject.Bussiness.DTOs.SupportsDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Profilies
{
    public class SupportMapProfilies : Profile
    {
        public SupportMapProfilies()
        {
            CreateMap<Support, SupportDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<CreateSupportDto, Support>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
