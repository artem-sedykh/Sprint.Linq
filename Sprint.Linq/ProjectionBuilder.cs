using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sprint.Linq
{
    public class ProjectionBuilder<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        public Expression<Func<TSource, TDestination>> DefaultProjection
        {
            get { return _defaultProjection; }
            set
            {
                _defaultProjection = value;

                _defaultProjectionExpression = value != null
                    ? Expression.Invoke(value, _parameter).Expand()
                    : null;
            }
        }

        private readonly IDictionary<MemberInfo, Expression> _bindings;
        private readonly ParameterExpression _parameter;
        private Expression<Func<TSource, TDestination>> _defaultProjection;
        private Expression _defaultProjectionExpression;

        public ProjectionBuilder()
        {
            _bindings = new Dictionary<MemberInfo, Expression>();

            _parameter = Expression.Parameter(typeof(TSource), "entity");
        }

        public ProjectionBuilder<TSource, TDestination> Register<TProperty>(Expression<Func<TSource, TProperty>> key,
            Expression<Func<TSource, TDestination>> binding)
        {
            var member = GetMemberInfo(key);

            _bindings.Add(member, Expression.Invoke(binding, _parameter).Expand());

            return this;
        }

        public Expression<Func<TSource, TDestination>> Build(params Expression<Func<TSource, object>>[] includes)
        {
            if (includes == null)
                return DefaultProjection;

            var bindings = includes.SelectMany(i => GetBindings(_bindings[GetMemberInfo(i)])).ToList();

            return Build(bindings);
        }

        public Expression<Func<TSource, TDestination>> Build(params string[] properties)
        {
            if (properties == null)
                return DefaultProjection;

            var bindings = properties.SelectMany(p =>
            {
                var binding = _bindings.First(b => String.Equals(p, b.Key.Name, StringComparison.OrdinalIgnoreCase));

                return GetBindings(binding.Value);
            }).ToList();

            return Build(bindings);
        }

        public Expression<Func<TSource, TDestination>> Build(Func<MemberInfo, bool> predicate = null)
        {
            if (predicate == null)
                return DefaultProjection;

            var bindings = _bindings.Where(b => predicate(b.Key)).SelectMany(b => GetBindings(b.Value)).ToList();

            return Build(bindings);
        }

        private Expression<Func<TSource, TDestination>> Build(List<MemberBinding> bindings)
        {
            if (DefaultProjection != null)
                bindings.AddRange(GetBindings(_defaultProjectionExpression));

            var newExpression = Expression.MemberInit(Expression.New(typeof(TDestination)), bindings);

            return Expression.Lambda<Func<TSource, TDestination>>(newExpression, _parameter);
        }

        private MemberInfo GetMemberInfo(LambdaExpression expression)
        {
            var member = (MemberExpression)expression.Body;

            return member.Member;
        }

        private ReadOnlyCollection<MemberBinding> GetBindings(Expression expression)
        {
            var memberInitExpression = (MemberInitExpression)expression;

            return memberInitExpression.Bindings;
        }
    }
}
