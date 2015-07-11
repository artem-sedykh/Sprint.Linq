using System.Linq.Expressions;

namespace Sprint.Linq
{
    internal interface IBuildMap        
    {
        LambdaExpression Build(string[] includes);
    }
}
