using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.TagDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public TagService(IRepository<Tag> tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task AddTagAsync(CreateTagDto createTagDto)
        {
            try
            {
                var existingTag = await _tagRepository.GetAsync(t => t.Name == createTagDto.Name);
                if (existingTag != null)
                {
                    throw new GlobalAppException("A tag with the same name already exists.");
                }
                var tag = _mapper.Map<Tag>(createTagDto);
                await _tagRepository.AddAsync(tag);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while adding the tag.", ex);
            }
        }

        public async Task<IEnumerable<TagGetDto>> GetAllTagsAsync()
        {
            try
            {
                var tags = await _tagRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TagGetDto>>(tags);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting all tags.", ex);
            }
        }
        public async Task<TagGetDto> GetTagByIdAsync(int id)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag == null)
                {
                    throw new GlobalAppException("Tag not found.");
                }
                return _mapper.Map<TagGetDto>(tag);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while getting the tag by ID.", ex);
            }
        }
    }
}
