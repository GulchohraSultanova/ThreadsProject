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
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;

        public TagService(ITagRepository tagRepository, IMapper mapper, IPostRepository postRepository)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _postRepository = postRepository;
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
                var tagDtos = _mapper.Map<IEnumerable<TagGetDto>>(tags);

                foreach (var tagDto in tagDtos)
                {
                    // Her bir tag için post sayısını hesapla
                    var postCount = await _postRepository.CountAsync(p => p.PostTags.Any(t => t.TagId == tagDto.Id));
                    tagDto.PostCount = postCount;
                }

                return tagDtos;
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
        public async Task DeleteTagAsync(int id)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag == null)
                {
                    throw new GlobalAppException("Tag not found.");
                }

                await _tagRepository.DeleteAsync(tag);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while deleting the tag.", ex);
            }
        }
        public async Task<IEnumerable<TagGetDto>> SearchTagsAsync(string searchTerm)
        {
            searchTerm = searchTerm.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                throw new GlobalAppException("Search term cannot be empty");
            }

            var tags = await _tagRepository.GetAllAsync(t => t.Name.StartsWith(searchTerm));
            var tagDtos = _mapper.Map<IEnumerable<TagGetDto>>(tags);

            foreach (var tagDto in tagDtos)
            {
                var postCount = await _postRepository.CountAsync(p => p.PostTags.Any(t => t.TagId == tagDto.Id));
                tagDto.PostCount = postCount;
            }

            return tagDtos;
        }

    }

}
