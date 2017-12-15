using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibTest.Data
{
    public class NHibernateHelper
    {
        private ISessionFactory _sessionFactory;

        public NHibernateHelper()
        {
            _sessionFactory = GetSessionFactory();
        }

        private ISessionFactory GetSessionFactory()
        {            
            var config = (new Configuration()).Configure();
            config.AddAssembly("NHibTest.Domain");
            return config.BuildSessionFactory();
        }

        public ISession GetSession()
        {
            return _sessionFactory.OpenSession();
        }
    }
}
