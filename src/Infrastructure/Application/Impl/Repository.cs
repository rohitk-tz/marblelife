using Core;
using Core.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Infrastructure.Application.Impl
{
    abstract class Repository
    {
        public abstract void SetDBContext(DbContext dbContext);
    }

    class Repository<T> : Repository, IRepository<T> where T: DomainBase
    {
        private DbContext _dbContext;
        DbSet<T> DbSet { get { return _dbContext.Set<T>(); } }

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Table
        {
            get
            {
                return DbSet.AsQueryable();
            }
        }
        public IQueryable<T> TableNoTracking
        {
            get
            {
                return DbSet.AsQueryable().AsNoTracking();
            }

        }

        public long Count(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression).Count();
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
            foreach (var entity in Fetch(expression))
            {
                Delete(entity);
            }
        }

        public void Delete(long id)
        {
            Delete(x => x.Id == id);
        }

        public void Delete(T entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            DbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<T> Fetch(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression).ToArray();
        }

        public IEnumerable<T> Fetch(Expression<Func<T, bool>> expression, int pageSize, int pageNumber)
        {
            return DbSet.Where(expression).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
        }

        public void Save(T entity)
        {
            if (entity.IsNew)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }

            _dbContext.SaveChanges();
        }

        public void Insert(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Added;
        }

        public void Update(T entity)
        {
            var original = DbSet.Find(entity.Id);

            _dbContext.Entry(original).CurrentValues.SetValues(entity);
            _dbContext.Entry(original).State = EntityState.Modified;

            (new SaveCascadeHelper()).SetCascadeOnModification(original, entity, _dbContext);
        }

        public T Get(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression).SingleOrDefault();
        }

        public T Get(long id)
        {
            return DbSet.Find(id);
        }

        public async Task<List<T>> TableAsync(Expression<Func<T, bool>> match)
        {
            return await DbSet.Where(match).ToListAsync();
        }

        public override void SetDBContext(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> IncludeMultiple(params Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                return includes.Aggregate(Table,
                          (current, include) => current.Include(include));
            }

            return Table;
        }
    }
}