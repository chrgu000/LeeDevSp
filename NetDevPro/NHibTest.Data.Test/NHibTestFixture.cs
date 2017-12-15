using NHibTest.Domain.Entities;
using NUnit.Framework;

namespace NHibTest.Data.Test
{
    [TestFixture]
    public class NHibTestFixture
    {
        private NHibTest _test;

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            var helper = new NHibernateHelper();
            _test = new NHibTest(helper.GetSession());
        }

        [Test]
        public void GetCustomerByIdTest()
        {
            var temp = new Customer { FirstName = "李", LastName = "建龙" };
            _test.CreateCustomer(temp);
            Customer customer = _test.GetCustomerById(1);
            int customerId = customer.Id;
            Assert.AreEqual(1, customer.Id);
        }
    }
}
