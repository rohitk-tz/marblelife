using System;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Application.Impl
{
    public interface ISortingHelper
    {
        IOrderedQueryable<M> ApplySorting<M, T>(IQueryable<M> queryFranchisee, Expression<Func<M, T>> sortExpression, long? sortOrder);
    }
}
