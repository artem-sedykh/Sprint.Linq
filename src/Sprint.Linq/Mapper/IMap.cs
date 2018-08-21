using System.Linq.Expressions;
// ReSharper disable once CheckNamespace

namespace Sprint.Linq
{
    internal interface IBuildMap
    {
        LambdaExpression Build(string[] includes);

        LambdaExpression BuildAll();

        LambdaExpression BuildAll(string[] excludeColumns);
    }
}
