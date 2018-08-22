using System.Linq;
using NUnit.Framework;
using Sprint.Linq.Test.Models;

namespace Sprint.Linq.Test
{
    [TestFixture]
    public class ExpressionDecoratorTest
    {
        public ExpressionEqualityComparer ExpressionComparer { get; set; }

        [SetUp]
        public void TestInitialize()
        {
            ExpressionComparer = new ExpressionEqualityComparer();
        }

        [Test]
        public void QueryableSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.QueryableCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.QueryableCustomers.Any(d => d.Id > 15))));
        }

        [Test]
        public void CollectionSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.CollectionCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.CollectionCustomers.Any(d => d.Id > 15))));
        }

        [Test]
        public void EnumerableSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.EnumerableCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.EnumerableCustomers.Any(d => d.Id > 15))));
        }

        [Test]
        public void ArraySource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(d => d.Id > 15))));

            Assert.IsTrue(ExpressionComparer.Equals(
              Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate())).Expand(),
              Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(d => d.Id > 15))));
        }

        [Test]
        public void ArrayQueryableEnumerableCollectionSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(expression) && c.EnumerableCustomers.Any(expression) && c.CollectionCustomers.Any(expression) && c.QueryableCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(d => d.Id > 15) && c.EnumerableCustomers.Any(d => d.Id > 15) && c.CollectionCustomers.Any(d => d.Id > 15) && c.QueryableCustomers.Any(d => d.Id > 15))));
        }

    }
}
