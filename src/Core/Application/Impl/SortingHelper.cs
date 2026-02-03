using Core.Application.Attribute;
using Core.Users.Enum;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Application.Impl
{
    [DefaultImplementation]
    public class SortingHelper : ISortingHelper
    {
        public IOrderedQueryable<M> ApplySorting<M, T>(IQueryable<M> sortQuery, Expression<Func<M, T>> sortExpression, long? sortOrder)
        {
            if (sortOrder == (long)SortingOrder.Asc)
            {
                return sortQuery.OrderBy(sortExpression);
            }
            return sortQuery.OrderByDescending(sortExpression);
        }
    }
}
