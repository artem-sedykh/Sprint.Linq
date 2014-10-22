namespace Sprint.Linq
{
    using System.Diagnostics;
    using System.Linq.Expressions;

    [DebuggerDisplay("{Expression}")]
    public class LambdaExpressionDecorator<TDelegate>
    {
        private readonly Expression<TDelegate> _expression;
        public Expression<TDelegate> Expression {
            get { return _expression; }
        }

        public LambdaExpressionDecorator(Expression<TDelegate> expression)
        {
            _expression = expression;
        }

        public static implicit operator TDelegate(LambdaExpressionDecorator<TDelegate> decorator)
        {

            return (decorator != null && decorator.Expression != null)
                ? decorator.Expression.Compile()
                : default(TDelegate);
        }

        public static implicit operator Expression<TDelegate>(LambdaExpressionDecorator<TDelegate> decorator)
        {
            return decorator.Expression;
        }
    }
}
