using NUnit.Framework;
using Sprint.Linq.Test.Models;

namespace Sprint.Linq.Test
{
    [TestFixture]
    public class PredicateBuilderTest
    {
        public ExpressionEqualityComparer ExpressionComparer { get; set; }

        [SetUp]
        public void TestInitialize()
        {
            ExpressionComparer = new ExpressionEqualityComparer();
        }

        [Test]
        public void Or()
        {
            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 15).Or(Linq.Expr<Customer, bool>(b => b.Id < 10)).Or(Linq.Expr<Customer, bool>(c => c.Name.Contains("test"))).Or(Linq.Expr<Customer, bool>(a => a.Id < 10)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 15 || a.Id < 10 || a.Name.Contains("test") || a.Id < 10)));

            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 1).Or(Linq.Expr<Customer, bool>(b => b.Id < 2)).Or(Linq.Expr<Customer, bool>(c => c.Name.Contains("3"))).Or(Linq.Expr<Customer, bool>(a => a.Id < 4)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 1 || a.Id < 2 || a.Name.Contains("3") || a.Id < 4)));

        }

        [Test]
        public void And()
        {
            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 15).And(Linq.Expr<Customer, bool>(b => b.Id < 10)).And(Linq.Expr<Customer, bool>(c => c.Name.Contains("test"))).And(Linq.Expr<Customer, bool>(a => a.Id < 10)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 15 && a.Id < 10 && a.Name.Contains("test") && a.Id < 10)));

            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 1).And(Linq.Expr<Customer, bool>(b => b.Id < 2)).And(Linq.Expr<Customer, bool>(c => c.Name.Contains("3"))).And(Linq.Expr<Customer, bool>(a => a.Id < 4)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 1 && a.Id < 2 && a.Name.Contains("3") && a.Id < 4)));
        }
    }
}
