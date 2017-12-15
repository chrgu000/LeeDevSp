using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConsoleAppDapper.Repositories
{
    public interface IRepository<T>
    {
        bool Add(T item);
        bool Delete(T item);
        bool Update(T item);
        T Query(int id);
        IEnumerable<T> Query(Expression<Predicate<T>> condition);
    }
}
