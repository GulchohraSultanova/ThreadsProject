using System.Collections.Generic;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.ActionDtos;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public interface IUserActionService
    {

        Task<IEnumerable<UserActionDto>> GetUserActionsAsync(string userId);
        Task CreateUserActionAsync(CreateUserActionDto createUserActionDto);
        Task<string> GetActionMessageAsync(UserActionDto userAction);
    }
}
