using System;
using System.Linq.Expressions;

namespace Sprint.Linq
{
    public interface IInitIExpressionMapper<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        IExpressionMapper<TSource, TDestination> DefaultMap(Expression<Func<TSource, TDestination>> projection);
    }

}
