using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Hubs
{
    public class LikeHub : Hub
    {
        public async Task SendLikeNotification(int postId, string userId)
        {
            await Clients.All.SendAsync("ReceiveLikeNotification", postId, userId);
        }
        public async Task SendUnlikeNotification(int postId, string userId)
        {
            await Clients.All.SendAsync("ReceiveUnlikeNotification", postId, userId);
        }
    }
}
