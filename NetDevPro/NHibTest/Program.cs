using NHibernate.Util;
using NHibTest.Data;
using NHibTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();
            test.GetCustomerByIdTest();
            test.TestHQL();

            "\r\n============================".Output();
            TestCriteria testCriteria = new TestCriteria();
            testCriteria.Test();

            "\r\n============================".Output();
            TestAUD testAUD = new TestAUD();
            testAUD.Test();

            "\r\n============================".Output();
            TestTransation testTransation = new TestTransation();
            testTransation.Test();

            Console.Read();
        }
    }

    class Test
    {
        private Data.NHibTest _test;

        public Test()
        {
            TestFixtureSetup();
        }

        public void TestFixtureSetup()
        {
            var helper = new NHibernateHelper();
            _test = new Data.NHibTest(helper.GetSession());
        }

        public void GetCustomerByIdTest()
        {
            var temp = new Customer { FirstName = "李", LastName = "建龙" };
            _test.CreateCustomer(temp);
            Customer customer = _test.GetCustomerById(1);
            int customerId = customer.Id;
            Console.WriteLine(customerId == 1);
        }

        private void Output<T>(string desc, IList<T> collection)
        {
            Console.WriteLine(desc);
            collection.ForEach(c =>
            {
                //Console.WriteLine(c);
                c.Output();
            });
        }

        public void FromTest()
        {
            var customers = _test.From();
            Output("==>FromTest:", customers);
        }

        public void FromAliasTest()
        {
            var customers = _test.FromAlias();
            Output("==>FromAliasTest:", customers);
        }

        public void SelectTest()
        {
            var customers = _test.Select();
            Output("==>SelectTest:", customers);
        }

        public void SelectObjectTest()
        {
            var customers = _test.SelectObject();
            Output("==>SelectObjectTest:", customers);
        }

        public void AggregateFunctionTest()
        {
            var customers = _test.AggregateFunction();
            Output("==>AggregateFunctionTest:", customers);
        }

        public void WhereTest()
        {
            var customers = _test.AggregateFunction();
            Output("==>WhereTest:", customers);
        }

        public void OrderByTest()
        {
            var customers = _test.OrderBy();
            Output("==>OrderByTest:", customers);
        }

        public void GroupByTest()
        {
            var customers = _test.GroupBy();
            Output("==>GroupByTest:", customers);
        }

        public void GetCustomersByFirstNameTest(string firstName)
        {
            var customers = _test.GetCustomersByFirstName(firstName);
            Output("==>GetCustomersByFirstNameTest:", customers);
        }

        public void GetCustomersWithIdGreaterThanTest(int id)
        {
            var customers = _test.GetCustomersWithIdGreaterThan(id);
            Output("==>GetCustomersWithIdGreaterThanTest:", customers);
        }

        public void TestHQL()
        {
            FromTest();
            FromAliasTest();
            SelectTest();
            SelectObjectTest();
            AggregateFunctionTest();
            WhereTest();
            OrderByTest();
            GroupByTest();
            GetCustomersByFirstNameTest("Jackie");
            GetCustomersWithIdGreaterThanTest(1);
        }
    }

    static class Extensions
    {
        public static void Output<T>(this string desc, IList<T> collection)
        {
            Console.WriteLine(desc);
            collection.ForEach(c =>
            {
                c.Output();
            });
        }

        public static void Output(this object obj)
        {
            if (obj is object[])
                (obj as object[]).Output();
            else
                Console.WriteLine(obj);
        }

        public static void Output(this object[] objs)
        {
            if (objs == null || objs.Length == 0)
                return;
            foreach (var obj in objs)
            {
                Console.Write("{0};", obj);
            }
            Console.WriteLine();
        }
    }

    class TestCriteria
    {
        private NHibCriteria _test;

        public TestCriteria()
        {
            _test = new NHibCriteria(new NHibernateHelper().GetSession());
        }

        private void CreateCriteriaTest()
        {
            var result = _test.CreateCriteria();
            "==>CreateCriteriaTest:".Output(result);
        }

        private void NarrowingTest()
        {
            var result = _test.Narrowing();
            "==>NarrowingTest:".Output(result);
        }

        private void OrderTest()
        {
            var result = _test.Order();
            "==>OrderTest:".Output(result);
        }

        private void QueryTest()
        {
            var result = _test.Query();
            "==>QueryTest:".Output(result);
        }

        private void UseQueryByExample_GetCustomerTest(Customer customer)
        {
            var result = _test.UseQueryByExample_GetCustomer(customer);
            "==>UseQueryByExample_GetCustomerTest:".Output(result);
        }

        private void GetCustomersByFirstNameAndLastNameTest(string firstName, string lastName)
        {
            var result = _test.GetCustomersByFirstNameAndLastName(firstName, lastName);
            "==>GetCustomersByFirstNameAndLastNameTest:".Output(result);
        }

        public void Test()
        {
            CreateCriteriaTest();
            NarrowingTest();
            OrderTest();
            QueryTest();
            UseQueryByExample_GetCustomerTest(new Customer { LastName = "Green" });
            GetCustomersByFirstNameAndLastNameTest("Harry", "Pott");
        }
    }

    class TestAUD
    {
        private NHibCriteria _test;
        private NHibAUD _testAUD;
        private int _newId = 0;

        public TestAUD()
        {
            _test = new NHibCriteria(new NHibernateHelper().GetSession());
            _testAUD = new NHibAUD();
        }

        private void CreateCustomerTest()
        {
            var result = _testAUD.CreateCustomer(new Customer { FirstName = "Jackie2", LastName = "Lee" });
            _newId = result;
            $"==>CreateCustomerTest:{result}".Output();
        }

        private void UpdateCustomerTest()
        {
            var customer = _test.GetCustomerById(_newId);
            customer.FirstName = $"Jackie{_newId}";
            _testAUD.UpdateCustomer(customer);
            "==>UpdateCustomerTest:".Output();
        }

        private void AddOrUpdateCustomerTest()
        {
            var customers = _test.GetCustomersByFirstName("李");
            foreach (var c in customers)
            {
                c.LastName = "Jianlong";
            }
            var c1 = new Customer { FirstName = "Jackie", LastName = "Jianlong" };
            var c2 = new Customer { FirstName = "Jackie", LastName = "Jianlong" };
            customers.Add(c1);
            customers.Add(c2);
            _testAUD.AddOrUpdateCustomer(customers);
            "==>AddOrUpdateCustomerTest:".Output(customers);
            "========================".Output();
            var list = _test.GetCustomersByFirstName("Jackie");
            "==>Jackie first names:".Output(list);
        }

        private void DeleteCustomerTest()
        {
            var customer = _test.GetCustomerById(_newId);
            _testAUD.DeleteCustomer(customer);
            "==>DeleteCustomerTest:".Output();
        }

        public void Test()
        {
            CreateCustomerTest();
            UpdateCustomerTest();
            AddOrUpdateCustomerTest();
            DeleteCustomerTest();
        }
    }
    class TestTransation
    {
        private NHibCriteria _test;
        private NHibTransaction _testTrans;
        private int _newId = 0;

        public TestTransation()
        {
            _test = new NHibCriteria(new NHibernateHelper().GetSession());
            _testTrans = new NHibTransaction();
        }

        private void CreateCustomerTransationTest()
        {
            var customer = new Customer { FirstName = "Jackie", LastName = "Lee" };
            int newIdentity = _testTrans.CreateCustomerTransation(customer);
            _newId = newIdentity;
            "==>CreateCustomerTransationTest:".Output();
        }

        public void DeleteCustomerTransactionTest()
        {
            var customer = _test.GetCustomerById(_newId);
            _testTrans.DeleteCustomerTransaction(customer);
            "==>DeleteCustomerTransactionTest:".Output();
        }

        public void UpdateCustomerTransactionTest()
        {
            var customer = _test.GetCustomerById(_newId);
            customer.FirstName = $"{customer.FirstName}-({customer.Id})";
            _testTrans.UpdateCustomerTransaction(customer);
            "==>UpdateCustomerTransactionTest:".Output();
        }

        public void SaveOrUpdateCustomersTransactionTest()
        {
            var customers = _test.GetCustomersByFirstName("李");
            foreach (var c in customers)
            {
                c.LastName = "Jianlong2";
            }
            var c1 = new Customer { FirstName = "Jackie55", LastName = "Lee" };
            customers.Add(c1);
            _testTrans.SaveOrUpdateCustomersTransaction(customers);
            "==>SaveOrUpdateCustomersTransactionTest:".Output();
        }

        public void Test()
        {
            CreateCustomerTransationTest();
            UpdateCustomerTransactionTest();
            SaveOrUpdateCustomersTransactionTest();
            DeleteCustomerTransactionTest();
        }
    }
}
