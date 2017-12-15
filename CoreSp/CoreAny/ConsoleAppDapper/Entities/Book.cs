using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppDapper.Entities
{
    /// <summary>
    /// 书
    /// </summary>
    public class Book : Entity
    {
        public string Name { get; set; }
        public List<BookReview> Reviews { get; set; }

        public Book()
        {
            Reviews = new List<Entities.BookReview>();
        }

        public override string ToString()
        {
            return $"[{Id}----{Name}]";
        }
    }
}
