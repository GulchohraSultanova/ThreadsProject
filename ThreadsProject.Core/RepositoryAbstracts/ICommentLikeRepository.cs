using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Core.RepositoryAbstracts
{
    public  interface ICommentLikeRepository:IRepository<CommentLike>
    {
        Task<CommentLike> GetCommentLikeAsync(int commentId, string userId);
    }
}
