using EcomApi.Application.Interfaces;
using EcomApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Infrastructure.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _appDbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _dbSet = appDbContext.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        //get by id
        public async Task<T> GetById(int id)
        {

            var result = await _dbSet.FindAsync(id);
            return result;
        }

        //get by id string
        public async Task<T> GetById(string id)
        {

            var result = await _dbSet.FindAsync(id);
            return result;
        }

        //add
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            // await _appDbContext.SaveChangesAsync();
            return entity;
        }


        //delete
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }

        //update
        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _appDbContext.Entry(entity).State = EntityState.Modified;
            //await _appDbContext.SaveChangesAsync();
            return entity;
        }

        //remove range
        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);


        }

        //to get list int id 
        public async Task<List<T>> GetByIdAsync(int id, Expression<Func<T, bool>> predicate)
        {

            var result = await _dbSet.Where(predicate).ToListAsync();
            return result;
        }

        //to get list int id
        public async Task<List<T>> GetByIdAsync(string id, Expression<Func<T, bool>> predicate)
        {

            var result = await _dbSet.Where(predicate).ToListAsync();
            return result;
        }

        //to get by id  expretion
        public async Task<T> GetById(string id, Expression<Func<T, bool>> predicate)
        {

            var result = await _dbSet.FirstOrDefaultAsync(predicate);
            return result;
        }
        //to get by id  expretion int id
        public async Task<T> GetById(int id, Expression<Func<T, bool>> predicate)
        {

            var result = await _dbSet.FirstOrDefaultAsync(predicate);
            return result;
        }


        //get by querry
        public IQueryable<T> GetAsyncQueryable(string id, Expression<Func<T, bool>> predicate)
        {

            IQueryable<T> result = _dbSet.Where(predicate);
            return result;
        }


        //get by querry int id
        public IQueryable<T> GetAsyncQueryable(int id, Expression<Func<T, bool>> predicate)
        {

            IQueryable<T> result = _dbSet.Where(predicate);
            return result;
        }

        // Retrieves a queryable collection
        public IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task SaveChanges()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}
