using NHibernate;
using NHibernate.Util;
using NHibTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibTest.Data
{
    public class NHibAUD
    {
        private ISession _session;
        public NHibAUD()
        {
            _session = new NHibernateHelper().GetSession();
        }

        public int CreateCustomer(Customer customer)
        {
            int newId = (int)_session.Save(customer);
            _session.Flush();
            return newId;
        }

        public void DeleteCustomer(Customer customer)
        {
            var cust = _session.Merge(customer);
            _session.Delete(cust);
            _session.Flush();
        }

        public void UpdateCustomer(Customer customer)
        {
            var cust = _session.Merge(customer);
            _session.Update(cust);
            _session.Flush();
        }

        public void AddOrUpdateCustomer(IList<Customer> customers)
        {
            customers.ForEach(c =>
            {
                _session.SaveOrUpdate(c);
            });
            _session.Flush();
        }
    }
}
