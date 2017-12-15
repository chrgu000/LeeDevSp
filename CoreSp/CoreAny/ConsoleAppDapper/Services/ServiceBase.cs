using ConsoleAppDapper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ConsoleAppDapper.Services
{
    public abstract class ServiceBase<T> : IService<T> where T : class, IEntity
    {
        public bool Add(T item)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Predicate<T>> query)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetList(Expression<Predicate<T>> query)
        {
            throw new NotImplementedException();
        }

        public T GetSingle(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(T item)
        {
            throw new NotImplementedException();
        }
    }
}
