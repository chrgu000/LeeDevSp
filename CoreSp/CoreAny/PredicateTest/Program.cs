using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PredicateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<User, bool>> predicate = e => true;
            List<User> users = new List<User>
            {
                new User { Id = 1, Name = "User1", Age = 21 },
                new User { Id = 2, Name = "User2", Age = 22 },
                new User { Id = 3, Name = "User3", Age = 23 },
                new User { Id = 4, Name = "User4", Age = 24 },
                new User { Id = 5, Name = "User5", Age = 25 },
                new User { Id = 6, Name = "User6", Age = 26 },
                new User { Id = 7, Name = "User7", Age = 27 },
                new User { Id = 8, Name = "User8", Age = 28 },
                new User { Id = 9, Name = "User9", Age = 29 },
                new User { Id = 10, Name = "User10", Age = 30 }
            };

            Expression<Func<User, bool>> predicate2 = e => e.Age > 23;
            predicate = (Expression<Func<User,bool>>)Expression.And(predicate, predicate2).Reduce();

            var res = users.AsQueryable().Where(predicate);
            foreach(var u in res)
            {
                u.ToString();
            }
        }
    }

    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{Id},{Name},{Age}\n";
        }
    }
}
