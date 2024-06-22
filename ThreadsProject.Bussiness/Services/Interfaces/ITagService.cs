using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.TagDto;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public  interface ITagService
    {
        Task AddTagAsync(CreateTagDto createTagDto);
        Task<IEnumerable<TagGetDto>> GetAllTagsAsync();
        Task<TagGetDto> GetTagByIdAsync(int id);
    }
}
