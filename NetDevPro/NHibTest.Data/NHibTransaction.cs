using NHibernate;
using NHibTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibTest.Data
{
    public class NHibTransaction
    {
        private ISession _session;

        public NHibTransaction()
        {
            _session = new NHibernateHelper().GetSession();
        }

        public int CreateCustomerTransation(Customer customer)
        {
            using (ITransaction trans = _session.BeginTransaction())
            {
                try
                {
                    int newId = (int)_session.Save(customer);
                    _session.Flush();
                    trans.Commit();
                    return newId;
                }
                catch (HibernateException)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void DeleteCustomerTransaction(Customer customer)
        {
            using (ITransaction trans = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(customer);
                    _session.Flush();
                    trans.Commit();
                }
                catch (HibernateException)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void UpdateCustomerTransaction(Customer customer)
        {
            using (ITransaction trans = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(customer);
                    _session.Flush();
                    trans.Commit();
                }
                catch (HibernateException)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void SaveOrUpdateCustomersTransaction(IList<Customer> customers)
        {
            using (ITransaction trans = _session.BeginTransaction())
            {
                try
                {
                    foreach (Customer c in customers)
                        _session.SaveOrUpdate(c);
                    _session.Flush();
                    trans.Commit();
                }
                catch (HibernateException)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}
