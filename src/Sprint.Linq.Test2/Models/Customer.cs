using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprint.Linq.Test.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Customer Parent { get; set; }

        public decimal Salary { get; set; }

        public ICollection<Customer> CollectionCustomers { get; set; }

        public IQueryable<Customer> QueryableCustomers { get; set; }

        public IEnumerable<Customer> EnumerableCustomers { get; set; }

        public Customer[] ArrayCustomers { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
