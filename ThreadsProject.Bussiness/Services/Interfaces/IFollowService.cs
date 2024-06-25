using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.FollowsDto;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public  interface IFollowService
    {
        Task FollowAsync(string senderId, string receiverId);
        Task AcceptFollowRequestAsync(int requestId);
        Task<IEnumerable<FollowRequestDto>> GetFollowRequestsAsync(string userId);
        Task<IEnumerable<FollowDto>> GetFollowersAsync(string userId);
        Task<IEnumerable<FollowDto>> GetFollowingsAsync(string userId);
        Task RejectFollowRequestAsync(int requestId);
        Task RemoveFollowerAsync(string userId, string followerId);
        Task RemoveFollowingAsync(string userId, string followerId);
        Task<IEnumerable<SentFollowRequestDto>> GetSentFollowRequestsAsync(string userId);
        Task CancelFollowRequestAsync(string senderId, string receiverId);
    }
}
