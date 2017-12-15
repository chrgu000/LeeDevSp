using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppDapper.Entities
{
    /// <summary>
    /// 书评
    /// </summary>
    public class BookReview : Entity
    {
        public int BookId { get; set; }
        public virtual string Content { get; set; }
        public virtual Book AssociationWithBook { get; set; }

        public override string ToString()
        {
            return $"{Id})--[{BookId}\t\"{Content}\"]";
        }
    }
}
