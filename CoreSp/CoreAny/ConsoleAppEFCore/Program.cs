using ConsoleAppEFCore.Model;
using ConsoleAppEFCore.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppEFCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (DbQueryCommit qc = new DbQueryCommit(new EFCoreContext()))
            {
                qc.Commit(() =>
                {
                    qc.Set<Blog>().Add(new Blog
                    {
                        Count = 1,
                        Name = "CheckingDemo",
                        Url = "http://welldone.org"
                    });
                });

                var q = qc.Query<Blog>(1);
                q.ConsoleWriteLine();
                               
            }

            var ef = new EFCoreContext();
            var blogs = ef.Blogs;

            var b1 = blogs.Skip(1).Take(1).ToList();
            var b2 = blogs.Skip(10).Take(10).ToList();

            Console.Read();
        }

        static void NoCheck(DbQueryCommit readerWriter1, DbQueryCommit readerWriter2, DbQueryCommit readerWriter3)
        {
            int id = 1;
            Blog blog1 = readerWriter1.Query<Blog>(id);
            Blog blog2 = readerWriter2.Query<Blog>(id);

            readerWriter1.Commit(() => blog1.Name = nameof(readerWriter1));
            readerWriter2.Commit(() => blog2.Name = nameof(readerWriter2));

            Blog blog3 = readerWriter3.Query<Blog>(id);
            Console.WriteLine(blog3.Name);
        }
    }

    static class Extensions
    {
        public static void ConsoleWriteLine(this object obj)
        {
            Console.WriteLine(obj);
        }
    }
}
