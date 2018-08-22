using NUnit.Framework;
using Sprint.Linq.Test.Models;

namespace Sprint.Linq.Test
{
    [TestFixture]
    public class MapperTest
    {
        [Test]
        public void Map()
        {
            Mapper.CreateMap<Company, CompanyView>()
                .DefaultMap(c => new CompanyView { Id = c.Id })
                .Include("Name", c => new CompanyView { Name = c.Name, Id = c.Id });

            var expression = Mapper.Map<Company, CompanyView>("Name");

            Assert.IsNotNull(expression);
        }
    }
}
