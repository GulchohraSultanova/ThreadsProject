using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.Data.RepositoryConcreters
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ThreadsContext _context;

        public Repository(ThreadsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException($"An error occurred while adding the {typeof(T).Name.ToLower()}.", ex);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new GlobalAppException($"An error occurred while deleting the {typeof(T).Name.ToLower()}.", ex);
            }
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                query = includes.Aggregate(query, (current, include) => current.Include(include));

                return await Task.FromResult(query);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException($"An error occurred while getting all {typeof(T).Name.ToLower()}s.", ex);
            }
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                query = includes.Aggregate(query, (current, include) => current.Include(include));

                return await query.FirstOrDefaultAsync(filter);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException($"An error occurred while getting the {typeof(T).Name.ToLower()} by condition.", ex);
            }
        }
        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException($"An error occurred while getting the {typeof(T).Name.ToLower()} by ID.", ex);
            }
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await _context.Set<T>().CountAsync(filter);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException($"An error occurred while counting {typeof(T).Name.ToLower()}s.", ex);
            }
        }


    }
}
