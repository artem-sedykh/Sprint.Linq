using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprint.Linq.Test.Models;

namespace Sprint.Linq.Test
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class CompanyView
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    [TestClass]
    public class MapperTest
    {     
        [TestMethod]
        public void Map()
        {
            Mapper.CreateMap<Company, CompanyView>()
                .DefaultMap(c => new CompanyView {Id = c.Id})
                .Include("Name", c => new CompanyView {Name = c.Name, Id=c.Id});

            var expression = Mapper.Map<Company, CompanyView>("Name");

            Assert.IsNotNull(expression);
        }
    }
}
