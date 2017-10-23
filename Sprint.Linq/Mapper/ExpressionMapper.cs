using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sprint.Linq
{
    internal class ExpressionMapper<TSource, TDestination> : IInitIExpressionMapper<TSource, TDestination>, IExpressionMapper<TSource, TDestination>, IBuildMap
        where TSource : class
        where TDestination : class
    {             
        private Expression _projection;
        private readonly ParameterExpression _parameter;
        private readonly IDictionary<string, Expression> _bindings;

        public ExpressionMapper()
        {
            _parameter = Expression.Parameter(typeof(TSource), "entity");
            _bindings = new Dictionary<string, Expression>();
        }

        public IExpressionMapper<TSource, TDestination> DefaultMap(Expression<Func<TSource, TDestination>> projection)
        {
            _projection = Expression.Invoke(projection, _parameter).Expand();

            return this;
        }

        public IExpressionMapper<TSource, TDestination> Include(string key, Expression<Func<TSource, TDestination>> projection)
        {
            _bindings.Add(key, Expression.Invoke(projection, _parameter).Expand());

            return this;
        }

        public LambdaExpression Build(string[] includes)
        {
            includes = includes ?? new string[0];            

            var bindings = _bindings.Where(b => includes.Contains(b.Key)).SelectMany(b => GetBindings(b.Value)).ToList();
         
            return Build(bindings);
        }

        private Expression<Func<TSource, TDestination>> Build(List<MemberBinding> bindings)
        {
            bindings.AddRange(GetBindings(_projection));

            bindings = bindings.DistinctBy(x => x.Member.MetadataToken).ToList();
              
            var newExpression = Expression.MemberInit(Expression.New(typeof(TDestination)), bindings);

            return Expression.Lambda<Func<TSource, TDestination>>(newExpression, _parameter);
            
        }

        private IEnumerable<MemberBinding> GetBindings(Expression expression)
        {
            var memberInitExpression = (MemberInitExpression)expression;

            return memberInitExpression.Bindings;
        }

        public LambdaExpression BuildAll()
        {
            var bindings = _bindings.SelectMany(b => GetBindings(b.Value)).ToList();

            return Build(bindings);
        }

        public LambdaExpression BuildAll(string[] excludeColumns)
        {
            excludeColumns = excludeColumns ?? new string[0];

            var bindings = _bindings.Where(b => !excludeColumns.Contains(b.Key)).SelectMany(b => GetBindings(b.Value)).ToList();

            return Build(bindings);
        }
    }
}
