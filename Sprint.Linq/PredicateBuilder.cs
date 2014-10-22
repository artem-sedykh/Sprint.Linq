namespace Sprint.Linq
{
    using System;
    using System.Linq.Expressions;

    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return CreateLogicalBinaryExpression(left, right, ExpressionType.OrElse);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return CreateLogicalBinaryExpression(left, right, ExpressionType.AndAlso);
        }

        private static Expression<Func<T, bool>> CreateLogicalBinaryExpression<T>(Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right, ExpressionType type)
        {            
            var parameters = left.Parameters;

            var invocationExpression = Expression.Invoke(right, parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.MakeBinary(type, left.Body, invocationExpression), parameters);
        }

        /// <summary>
        /// Building a strongly typed lambda expression to calculate the intersections of intervals.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TProperty">Type field.</typeparam>
        /// <param name="firstIntervalBegin">The left-most boundary of the first interval.</param>
        /// <param name="firstIntervalEnd">The extreme right edge of the first interval.</param>
        /// <param name="secondIntervalBegin">The left-most boundary of the second interval.</param>
        /// <param name="secondIntervalEnd">The extreme right edge of the second interval.</param>
        /// <returns>Expression tree.</returns>
        public static Expression<Func<TSource, bool>> Intersection<TSource, TProperty>(Expression<Func<TSource, TProperty>> firstIntervalBegin, Expression<Func<TSource, TProperty>> firstIntervalEnd, TProperty secondIntervalBegin, TProperty? secondIntervalEnd) where TProperty : struct, IComparable, IComparable<TProperty>, IEquatable<TProperty>
        {
            if (firstIntervalBegin == null)
                throw new ArgumentNullException("firstIntervalBegin");

            if (firstIntervalEnd == null)
                throw new ArgumentNullException("firstIntervalEnd");

            var type = typeof(TProperty);
            var parametr = Expression.Parameter(typeof(TSource), "p");
            var expressionFirstIntervalBegin = Expression.Invoke(firstIntervalBegin, parametr);
            var expressionFirstIntervalEnd = Expression.Invoke(firstIntervalEnd, parametr);

            var expressionSecondIntervalBegin = Expression.Constant(secondIntervalBegin, type);
            var expressionSecondIntervalEnd = Expression.Constant(secondIntervalEnd, type);

            var expression =
                Expression.OrElse(
                    Expression.OrElse(
                        Expression.AndAlso(
                            Expression.GreaterThanOrEqual(expressionFirstIntervalBegin,
                                                          expressionSecondIntervalBegin),
                            Expression.LessThanOrEqual(expressionFirstIntervalBegin, expressionSecondIntervalEnd)),
                        Expression.AndAlso(
                            Expression.GreaterThanOrEqual(expressionFirstIntervalEnd, expressionSecondIntervalBegin),
                            Expression.LessThanOrEqual(expressionFirstIntervalEnd, expressionSecondIntervalEnd))),
                    Expression.AndAlso(
                        Expression.LessThanOrEqual(expressionFirstIntervalBegin, expressionSecondIntervalBegin),
                        Expression.GreaterThanOrEqual(expressionFirstIntervalEnd, expressionSecondIntervalEnd)));

            return Expression.Lambda<Func<TSource, bool>>(expression, parametr).Expand();
        }

        /// <summary>
        /// Building a strongly typed lambda expression to calculate the intersections of intervals.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TProperty">Type field.</typeparam>        
        /// <param name="firstIntervalEnd">The extreme right edge of the first interval.</param>
        /// <param name="secondIntervalBegin">The left-most boundary of the second interval.</param>        
        /// <returns>Expression tree.</returns>
        public static Expression<Func<TSource, bool>> IsIntersectionWithBegin<TSource, TProperty>(Expression<Func<TSource, TProperty>> firstIntervalEnd, TProperty secondIntervalBegin)
        {
            if (firstIntervalEnd == null)
                throw new ArgumentNullException("firstIntervalEnd");

            var type = typeof(TProperty);
            var parametr = Expression.Parameter(typeof(TSource), "p");
            var expressionFirstIntervalEnd = Expression.Invoke(firstIntervalEnd, parametr);

            var expressionSecondIntervalBegin = Expression.Constant(secondIntervalBegin, type);
            return Expression.Lambda<Func<TSource, bool>>(Expression.GreaterThanOrEqual(expressionFirstIntervalEnd, expressionSecondIntervalBegin), parametr).Expand();
        }

        /// <summary>
        /// Building a strongly typed lambda expression to calculate the intersections of intervals.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TProperty">Type field.</typeparam>    
        /// <param name="firstIntervalBegin">the leftmost boundary of the first interval.</param>        
        /// <param name="secondIntervalEnd">The extreme right edge of the second interval.</param>
        /// <returns>Expression tree.</returns>
        public static Expression<Func<TSource, bool>> IsIntersectionWithEnd<TSource, TProperty>(Expression<Func<TSource, TProperty>> firstIntervalBegin, TProperty secondIntervalEnd)
        {
            if (firstIntervalBegin == null)
                throw new ArgumentNullException("firstIntervalBegin");

            var type = typeof(TProperty);
            var parametr = Expression.Parameter(typeof(TSource), "p");
            var expressionFirstIntervalBegin = Expression.Invoke(firstIntervalBegin, parametr);

            var expressionSecondIntervalEnd = Expression.Constant(secondIntervalEnd, type);
            return Expression.Lambda<Func<TSource, bool>>(Expression.LessThanOrEqual(expressionFirstIntervalBegin, expressionSecondIntervalEnd), parametr).Expand();
        }
    }
}
