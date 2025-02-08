using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetById(int id);

        Task<T> GetById(string id);
        Task<T> AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task<List<T>> GetByIdAsync(int id, Expression<Func<T, bool>> predicate);
        Task<List<T>> GetByIdAsync(string id, Expression<Func<T, bool>> predicate);
        Task<T> GetById(string id, Expression<Func<T, bool>> predicate);
        Task<T> GetById(int id, Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAsyncQueryable(string id, Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAsyncQueryable(int id, Expression<Func<T, bool>> predicate);
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate);
        Task SaveChanges();




    }
}
