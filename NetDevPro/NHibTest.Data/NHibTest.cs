using NHibernate;
using NHibTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibTest.Data
{
    public class NHibTest
    {
        public ISession Session { get; set; }
        public NHibTest(ISession session)
        {
            Session = session;
        }

        public void CreateCustomer(Customer customer)
        {
            Session.Save(customer);
            Session.Flush();
        }

        public Customer GetCustomerById(int customerId)
        {
            return Session.Get<Customer>(customerId);
        }

        public IList<Customer> From()
        {
            // 返回所有Customer类实例
            return Session.CreateQuery("from Customer").List<Customer>();
        }

        public IList<Customer> FromAlias()
        {
            // 返回所有Customer类实例，Customer赋予了别名customer
            return Session.CreateQuery("from Customer as customer").List<Customer>();
        }

        public IList<int> Select()
        {
            // 返回所有Customer的Id
            return Session.CreateQuery("select c.Id from Customer c").List<int>();
        }

        public IList<object[]> SelectObject()
        {
            return Session.CreateQuery("select c.FirstName, count(c.FirstName) from Customer c group by c.FirstName")
                .List<object[]>();
        }

        public IList<object[]> AggregateFunction()
        {
            return Session.CreateQuery("select avg(c.Id),sum(c.Id),count(c) from Customer c").List<object[]>();
        }

        public IList<Customer> Where()
        {
            return Session.CreateQuery("from Customer c where c.FirstName='Jackie' ").List<Customer>();
        }

        public IList<Customer> OrderBy()
        {
            return Session.CreateQuery("from Customer c order by c.FirstName asc,c.LastName desc").List<Customer>();
        }

        public IList<object[]> GroupBy()
        {
            return Session.CreateQuery("select c.FirstName,count(c.FirstName) from Customer c group by c.FirstName").List<object[]>();
        }

        public IList<Customer> GetCustomersByFirstName(string firstName)
        {
            // 写法1
            //return Session.CreateQuery("from Customer c where c.FirstName='" + firstName + "' ").List<Customer>();

            // 写法2：位置参数
            /*return Session.CreateQuery("from Customer c where c.FirstName=?")
                .SetString(0,firstName)
                .List<Customer>();*/

            // 写法3：命名参数（推荐）
            return Session.CreateQuery("from Customer c where c.FirstName=:firstName")
                .SetString("firstName", firstName)
                .List<Customer>();
        }

        public IList<Customer> GetCustomersWithIdGreaterThan(int id)
        {
            return Session.CreateQuery("from Customer c where c.Id>:cid")
                .SetInt32("cid", id)
                .List<Customer>();
        }
    }
}
