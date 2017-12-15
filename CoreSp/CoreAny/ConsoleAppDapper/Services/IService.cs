using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConsoleAppDapper.Services
{
    public interface IService<T>
    {
        T GetSingle(int id);
        IEnumerable<T> GetList(Expression<Predicate<T>> query);
        int Count(Expression<Predicate<T>> query);
        bool Add(T item);
        bool Delete(T item);
        bool Delete(int id);
        bool Update(T item);
    }
}
