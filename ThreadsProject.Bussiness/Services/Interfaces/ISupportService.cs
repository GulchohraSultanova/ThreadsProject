using System.Collections.Generic;
using System.Threading.Tasks;

using ThreadsProject.Bussiness.DTOs.SupportsDto;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public interface ISupportService
    {
        Task CreateSupportAsync(CreateSupportDto createSupportDto, string userId);
     
    }
}
