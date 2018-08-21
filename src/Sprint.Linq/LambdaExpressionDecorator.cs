namespace Sprint.Linq
{
    using System.Diagnostics;
    using System.Linq.Expressions;

    [DebuggerDisplay("{" + nameof(Expression) + "}")]
    public class LambdaExpressionDecorator<TDelegate>
    {
        public Expression<TDelegate> Expression { get; }

        public LambdaExpressionDecorator(Expression<TDelegate> expression)
        {
            Expression = expression;
        }

        public static implicit operator TDelegate(LambdaExpressionDecorator<TDelegate> decorator)
        {
            return decorator?.Expression != null ? decorator.Expression.Compile() : default(TDelegate);
        }

        public static implicit operator Expression<TDelegate>(LambdaExpressionDecorator<TDelegate> decorator)
        {
            return decorator.Expression;
        }
    }
}
