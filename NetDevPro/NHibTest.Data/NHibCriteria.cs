using NHibernate;
using NHibernate.Criterion;
using NHibTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibTest.Data
{
    public class NHibCriteria
    {
        public ISession Session { get; set; }
        public NHibCriteria(ISession session)
        {
            Session = session;
        }

        public IList<Customer> CreateCriteria()
        {
            ICriteria crit = Session.CreateCriteria(typeof(Customer));
            crit.SetMaxResults(50);
            IList<Customer> customers = crit.List<Customer>();
            return customers;
        }

        public IList<Customer> Narrowing()
        {
            IList<Customer> customers = Session.CreateCriteria(typeof(Customer))
                .Add(Restrictions.Like("FirstName", "Jackie%"))
                .Add(Restrictions.Between("LastName", "A%", "J%"))
                .List<Customer>();
            return customers;
        }

        public IList<Customer> Order()
        {
            return Session.CreateCriteria(typeof(Customer))
                .Add(Restrictions.Like("FirstName", "J%"))
                .AddOrder(new Order("FirstName", false))
                .AddOrder(new Order("LastName", true))
                .List<Customer>();
        }

        public IList<Customer> Query()
        {
            Customer customer = new Customer { FirstName = "Jackie", LastName = "Lee" };
            return Session.CreateCriteria(typeof(Customer))
                .Add(Example.Create(customer))
                .List<Customer>();
        }

        public IList<Customer> UseQueryByExample_GetCustomer(Customer customer)
        {
            Example example = Example.Create(customer)
                .IgnoreCase()
                .EnableLike()
                .SetEscapeCharacter('&');
            return Session.CreateCriteria(typeof(Customer))
                .Add(example)
                .List<Customer>();
        }

        public IList<Customer> GetCustomersByFirstNameAndLastName(string firstName, string lastName)
        {
            return Session.CreateCriteria(typeof(Customer))
                .Add(Restrictions.Eq("FirstName", firstName))
                .Add(Restrictions.Eq("LastName", lastName))
                .List<Customer>();
        }

        public Customer GetCustomerById(int id)
        {
            return Session.CreateCriteria(typeof(Customer))
                .Add(Restrictions.Eq("Id", id))
                .List<Customer>().FirstOrDefault();
        }

        public IList<Customer> GetCustomersByFirstName(string firstName)
        {
            return Session.CreateCriteria(typeof(Customer))
                .Add(Restrictions.Eq("FirstName", firstName))
                .List<Customer>();
        }
    }
}
