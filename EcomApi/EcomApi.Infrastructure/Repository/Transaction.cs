using EcomApi.Application.Interfaces;
using EcomApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Infrastructure.Repository
{
    public class Transaction : ITransaction
    {
        private readonly AppDbContext _appDbContext;
        private IDbContextTransaction _transaction;
        private bool disposed = false;
        private readonly Dictionary<Type, object> _repossitores;
        public Transaction(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _repossitores = new Dictionary<Type, object>();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _appDbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {

            if (!disposed)
            {
                if (disposing)
                {
                    _appDbContext.Dispose();
                }
                disposed = true;
            }
        }
        public IBaseRepository<T> GetRepository<T>() where T : class
        {
            if (_repossitores.ContainsKey(typeof(T)))
            {
                return _repossitores[typeof(T)] as IBaseRepository<T>;
            }
            var repository = new BaseRepository<T>(_appDbContext);
            _repossitores.Add(typeof(T), repository);
            return repository;
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
