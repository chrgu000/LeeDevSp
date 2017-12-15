using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    // EF Core 并发
    class Program5
    {
        static void Main(string[] args)
        {
            //var sd=  Path.GetExtension("33.jpg");

            using (var context = new EducationContext())
            {
                // add
                context.Students.Add(new Student
                {
                    FirstName = "Jackie",
                    LastName = "Lee",
                    RollNumber = "1"
                });
                context.SaveChanges();

                Thread.Sleep(2000); // 让保存动作完成
                // update 并发5线程更改
                for (int i = 1; i <= 5; ++i)
                {
                    Task.Run(() =>
                    {
                        var stud = context.Students.FirstOrDefaultAsync(s => s.StudentId == 1).Result;
                        if (stud != null)
                        {
                            stud.RollNumber = $"{i + 1}";
                            context.SaveChanges(); // System.InvalidOperationException:“A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe.”
                        }
                    });
                }

                Console.WriteLine("Database Created!!!");


                Console.Read();
            }
        }
    }

    class EducationContext : DbContext
    {
        public EducationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=ConcurrencyCheck;Integrated Security=SSPI");
        }

        public DbSet<Student> Students { get; set; }
    }

    class Student
    {
        public int StudentId { get; set; }

        [ConcurrencyCheck]
        public string RollNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
