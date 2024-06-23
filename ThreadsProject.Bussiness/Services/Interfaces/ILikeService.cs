using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public  interface ILikeService
    {
        Task LikePostAsync(int postId, string userId);
        Task UnlikePostAsync(int postId, string userId);
    }
}
