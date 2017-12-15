using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppEFCore.Model.Entities
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}, Name:{Name}, Count:{Count}, Url:{Url}";
        }
    }
}
