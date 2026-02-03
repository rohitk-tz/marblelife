using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
    public interface IRepository<T> where T : DomainBase
    {
        T Get(long id);
        T Get(Expression<Func<T, bool>> expression);

        IEnumerable<T> Fetch(Expression<Func<T, bool>> expression);
        IEnumerable<T> Fetch(Expression<Func<T, bool>> expression, int pageSize, int pageNumber);

        long Count(Expression<Func<T, bool>> expression);
        void Save(T entity);

        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> expression);
        void Delete(long id);

        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }

        Task<List<T>> TableAsync(Expression<Func<T, bool>> match);
        IQueryable<T> IncludeMultiple(params Expression<Func<T, object>>[] includes);
    }
}
