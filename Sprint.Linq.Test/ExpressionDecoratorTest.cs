using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprint.Linq.Test.Models;

namespace Sprint.Linq.Test
{
    [TestClass]
    public class ExpressionDecoratorTest
    {
        public ExpressionEqualityComparer ExpressionComparer { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ExpressionComparer = new ExpressionEqualityComparer();
        }

        [TestMethod]
        public void QueryableSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();
            
            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.QueryableCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.QueryableCustomers.Any(d => d.Id > 15))));
        }

        [TestMethod]
        public void CollectionSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.CollectionCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.CollectionCustomers.Any(d => d.Id > 15))));
        }

        [TestMethod]
        public void EnumerableSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.EnumerableCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.EnumerableCustomers.Any(d => d.Id > 15))));
        }

        [TestMethod]
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

        [TestMethod]
        public void ArrayQueryableEnumerableCollectionSource()
        {
            var expression = Linq.Expr<Customer, bool>(d => d.Id > 15).Decorate();

            Assert.IsTrue(ExpressionComparer.Equals(
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(expression) && c.EnumerableCustomers.Any(expression) && c.CollectionCustomers.Any(expression) && c.QueryableCustomers.Any(expression)).Expand(),
               Linq.Expr<Customer, bool>(c => c.Id > 0 && c.ArrayCustomers.Any(d => d.Id > 15) && c.EnumerableCustomers.Any(d => d.Id > 15) && c.CollectionCustomers.Any(d => d.Id > 15) && c.QueryableCustomers.Any(d => d.Id > 15))));
        }
    }
}
