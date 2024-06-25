using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.RepostDto;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public interface IRepostService
    {
        Task AddRepostAsync(RepostCreateDto repostCreateDto, string userId);
        Task RemoveRepostAsync(int repostId, string userId);
        Task<RepostGetDto> GetRepostByIdAsync(int repostId);
        Task<IEnumerable<RepostGetDto>> GetAllRepostsAsync();
    }
}
