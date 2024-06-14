using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.RepositoryAbstracts
{
   public  interface IRepository<T> where T : class

    {
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T,bool>>? filter=null, params string[] includes);
        Task<T> GetAsync(Expression<Func<T, bool>> ? filter=null , params string[] includes);
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
      
    }
}
