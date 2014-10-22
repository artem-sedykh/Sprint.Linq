namespace Sprint.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionExpander
    {
        private static readonly Type ExpressionDecoratorType = typeof(LambdaExpressionDecorator<>);

        internal static Expression<TDelegate> Expand<TDelegate>(Expression<TDelegate> expression)
        {
            var expr = Visit(expression, null);

            return expr != null ? (Expression<TDelegate>)expr : null;
        }

        internal static Expression Visit(Expression exp, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            if (exp == null)
                return null;

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp, parameters);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)exp, parameters);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp, parameters);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp, parameters);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp, parameters);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp, parameters);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp, parameters);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp, parameters);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp, parameters);
                case ExpressionType.New:
                    return VisitNew((NewExpression)exp, parameters);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp, parameters);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp, parameters);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp, parameters);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        internal static MemberBinding VisitBinding(MemberBinding binding, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding, parameters);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding, parameters);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding, parameters);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        internal static ElementInit VisitElementInitializer(ElementInit initializer, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var arguments = VisitExpressionList(initializer.Arguments, parameters);
            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        internal static Expression VisitUnary(UnaryExpression node, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            if (node.NodeType == ExpressionType.Convert && node.Operand.Type.IsGenericType && node.Operand.Type.GetGenericTypeDefinition() == ExpressionDecoratorType)
            {
                var decorator = Expression.Lambda<Func<object>>(node.Operand).Compile()();

                var expressionProperty = decorator.GetType().GetProperty("Expression", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                var expr = (Expression)expressionProperty.GetValue(decorator, null);

                return expr;
            }

            return BaseVisitUnary(node, parameters);
        }
        internal static Expression BaseVisitUnary(UnaryExpression u, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var operand = Visit(u.Operand, parameters);

            return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
        }

        internal static Expression VisitBinary(BinaryExpression b, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var left = Visit(b.Left, parameters);
            var right = Visit(b.Right, parameters);
            var conversion = Visit(b.Conversion, parameters);

            if (left == b.Left && right == b.Right && conversion == b.Conversion) return b;

            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                return Expression.Coalesce(left, right, conversion as LambdaExpression);

            return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
        }

        internal static Expression VisitTypeIs(TypeBinaryExpression b, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var expr = Visit(b.Expression, parameters);

            return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
        }

        internal static Expression VisitConstant(ConstantExpression c, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            return c;
        }

        internal static Expression VisitConditional(ConditionalExpression c, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var test = Visit(c.Test, parameters);
            var ifTrue = Visit(c.IfTrue, parameters);
            var ifFalse = Visit(c.IfFalse, parameters);

            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
                return Expression.Condition(test, ifTrue, ifFalse);

            return c;
        }

        internal static Expression VisitParameter(ParameterExpression p, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            return (parameters != null && parameters.ContainsKey(p)) ? parameters[p] : p;            
        }

        internal static Expression VisitMemberAccess(MemberExpression m, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var exp = Visit(m.Expression, parameters);
            return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
        }

        internal static Expression VisitMethodCall(MethodCallExpression m, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var obj = Visit(m.Object, parameters);

            IEnumerable<Expression> args = VisitExpressionList(m.Arguments, parameters);

            if (obj != m.Object || args != m.Arguments)
                return Expression.Call(obj, m.Method, args);
            return m;
        }

        internal static ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = Visit(original[i], parameters);
                if (list != null)
                    list.Add(p);
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            return list != null ? list.AsReadOnly() : original;
        }

        internal static MemberAssignment VisitMemberAssignment(MemberAssignment assignment, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var e = Visit(assignment.Expression, parameters);

            return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
        }

        internal static MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var bindings = VisitBindingList(binding.Bindings, parameters);

            return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        internal static MemberListBinding VisitMemberListBinding(MemberListBinding binding, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var initializers = VisitElementInitializerList(binding.Initializers, parameters);
            return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
        }

        internal static IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var b = VisitBinding(original[i], parameters);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        internal static IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var init = VisitElementInitializer(original[i], parameters);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        internal static Expression VisitLambda(LambdaExpression lambda, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            var body = Visit(lambda.Body, parameters);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        internal static NewExpression VisitNew(NewExpression nex, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            IEnumerable<Expression> args = VisitExpressionList(nex.Arguments, parameters);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);

                return Expression.New(nex.Constructor, args);
            }
            return nex;
        }

        internal static Expression VisitMemberInit(MemberInitExpression init, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            NewExpression n = VisitNew(init.NewExpression, parameters);
            IEnumerable<MemberBinding> bindings = VisitBindingList(init.Bindings, parameters);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }

        internal static Expression VisitListInit(ListInitExpression init, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            NewExpression n = VisitNew(init.NewExpression, parameters);
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(init.Initializers, parameters);
            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return init;
        }

        internal static Expression VisitNewArray(NewArrayExpression na, IDictionary<ParameterExpression, ParameterExpression> parameters)
        {
            IEnumerable<Expression> exprs = VisitExpressionList(na.Expressions, parameters);

            if (exprs != na.Expressions)
                return na.NodeType == ExpressionType.NewArrayInit ? Expression.NewArrayInit(na.Type.GetElementType(), exprs) : Expression.NewArrayBounds(na.Type.GetElementType(), exprs);

            return na;
        }

        internal static Expression VisitInvocation(InvocationExpression node)
        {

            if (node.Expression.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)node.Expression;

                var parameters = node.Arguments.Select((e, i) => new
                {
                    Parameter = e,
                    ReplaceParameter = lambda.Parameters[i]
                }).ToDictionary(x => x.ReplaceParameter, x => (ParameterExpression)x.Parameter);

                var expr = ((LambdaExpression)Visit(node.Expression, parameters)).Body;

                return expr;
            }

            return BaseVisitInvocation(node);
        }

        internal static Expression BaseVisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = VisitExpressionList(iv.Arguments, null);
            var expr = Visit(iv.Expression, null);
            if (args != iv.Arguments || expr != iv.Expression)
            {
                return Expression.Invoke(expr, args);
            }
            return iv;
        }        
    }
}
