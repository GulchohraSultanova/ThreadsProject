using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ThreadsProject.Bussiness.DTOs.SupportsDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class SupportService : ISupportService
    {
        private readonly ISupportRepository _supportRepository;
        private readonly IMapper _mapper;

        public SupportService(ISupportRepository supportRepository, IMapper mapper)
        {
            _supportRepository = supportRepository;
            _mapper = mapper;
        }

        public async Task CreateSupportAsync(CreateSupportDto createSupportDto, string userId)
        {
            var support = _mapper.Map<Support>(createSupportDto);
            support.UserId = userId;
            await _supportRepository.AddAsync(support);
        }

      
    }
}
