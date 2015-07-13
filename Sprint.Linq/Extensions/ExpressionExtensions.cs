using System;
using System.Linq.Expressions;

namespace Sprint.Linq
{
    public static class ExpressionExtensions
    {
        public static LambdaExpressionDecorator<Func<T, bool>> Decorate<T>(this Expression<Func<T, bool>> expression)
        {
            return new LambdaExpressionDecorator<Func<T, bool>>(expression);
        }

        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expression)
        {
            return expression != null ? ExpressionExpander.Expand(expression) : null;
        }

        public static Expression Expand(this Expression expression)
        {
            return expression != null ? ExpressionExpander.Visit(expression, null) : null;
        }        
    }
}
