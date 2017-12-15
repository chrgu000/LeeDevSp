using ConsoleAppDapper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppDapper.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Book QueryBook(int id);

        bool DeleteBoth(int id);
    }
}
