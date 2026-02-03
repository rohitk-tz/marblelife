using Core;
using Core.Application;
using ORM;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;

namespace Infrastructure.Application.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;
        private IList<object> _repositories;
        private System.Data.Common.DbConnection _dbConnection;
        private DbTransaction _transaction;

        public UnitOfWork()
        {
            _dbContext = new MakaluDbContext();
            _repositories = new List<object>();
        }

        public void Setup()
        {
            if (_dbContext == null)
            {
                _dbContext = new MakaluDbContext();
                _repositories = new List<object>();
            }
        }

        public IRepository<T> Repository<T>() where T : DomainBase
        {
            var repo = (IRepository<T>)_repositories.SingleOrDefault(x => x is IRepository<T>);

            if (repo == null)
            {
                repo = new Repository<T>(_dbContext);
                _repositories.Add(repo);
            }

            if (_repositories.Count() == 1)
            {
                _dbConnection = _dbContext.Database.Connection;
                StartTransaction();
            }

            return repo;
        }

        public void SaveChanges()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction.Dispose();
                    _dbContext.Database.UseTransaction(null);
                }
            }
            finally
            {
                _transaction = null;
            }
        }

        public void StartTransaction()
        {
            if (_dbConnection.State == System.Data.ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            if (_transaction == null)
            {
                _transaction = _dbConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                _dbContext.Database.UseTransaction(_transaction);
            }
        }

        public void Dispose()
        {
            Cleanup();
        }

        public void Cleanup()
        {
            if (_transaction != null) _transaction.Rollback();
            _transaction = null;

            _dbContext.Dispose();
            _dbContext = null;            

            if (_dbConnection != null)
            {
                if (_dbConnection.State == System.Data.ConnectionState.Open)
                    _dbConnection.Close();

                _dbConnection.Dispose();
                _dbConnection = null;
            }

        }


        public void ResetContext()
        {
            _dbContext.Dispose();
            _dbContext = null;
            
            _dbContext = new MakaluDbContext();
            _dbConnection = _dbContext.Database.Connection;
            foreach (var repo in _repositories)
            {
                (repo as Repository).SetDBContext(_dbContext);
            }            
        }

        public void Rollback()
        {
            try
            {

                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _dbContext.Database.UseTransaction(null);
                }
            }
            finally
            {
                _transaction = null;
            }
        }
    }
}
