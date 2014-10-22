using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprint.Linq.Test.Models;

namespace Sprint.Linq.Test
{
    [TestClass]
    public class PredicateBuilderTest
    {
        public ExpressionEqualityComparer ExpressionComparer { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ExpressionComparer = new ExpressionEqualityComparer();
        }

        [TestMethod]
        public void Or()
        {
            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 15).Or(Linq.Expr<Customer, bool>(b => b.Id < 10)).Or(Linq.Expr<Customer, bool>(c => c.Name.Contains("test"))).Or(Linq.Expr<Customer, bool>(a => a.Id < 10)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 15 || a.Id < 10 || a.Name.Contains("test") || a.Id<10)));

            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 1).Or(Linq.Expr<Customer, bool>(b => b.Id < 2)).Or(Linq.Expr<Customer, bool>(c => c.Name.Contains("3"))).Or(Linq.Expr<Customer, bool>(a => a.Id < 4)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 1 || a.Id < 2 || a.Name.Contains("3") || a.Id < 4)));

        }

        [TestMethod]
        public void And()
        {
            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 15).And(Linq.Expr<Customer, bool>(b => b.Id < 10)).And(Linq.Expr<Customer, bool>(c => c.Name.Contains("test"))).And(Linq.Expr<Customer, bool>(a => a.Id < 10)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 15 && a.Id < 10 && a.Name.Contains("test") && a.Id < 10)));

            Assert.IsTrue(ExpressionComparer.Equals(
                Linq.Expr<Customer, bool>(a => a.Id > 1).And(Linq.Expr<Customer, bool>(b => b.Id < 2)).And(Linq.Expr<Customer, bool>(c => c.Name.Contains("3"))).And(Linq.Expr<Customer, bool>(a => a.Id < 4)).Expand(),
                Linq.Expr<Customer, bool>(a => a.Id > 1 && a.Id < 2 && a.Name.Contains("3") && a.Id < 4)));
        }

        [TestMethod]
        public void IntersectionTest()
        {                                   
            //var expression = PredicateBuilder.Intersection<Customer, DateTime>(x => x.StartDate, x => x.EndDate,
            //    DateTime.Now.AddYears(-10), DateTime.Now);

            //var originalExpression=
        }
    }
}
