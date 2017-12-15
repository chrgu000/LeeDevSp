using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Program4
    {
        static void Main01(string[] args)
        {
            Base b = new Base { Name = "Base Name" };
            Person p = new Person { Name = "Person Name", Address = "GD", Age = 20 };
            Student s = new Student { Name = "Student Name", Age = 18, Address = "AGD", Grade = 2, Class = 1 };

            Console.WriteLine(ToJson(b));
            Console.WriteLine(ToJson(p));
            Console.WriteLine(ToJson(s));

            Console.Read();
        }

        static string ToJson(Base b)
        {
            var props = b.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var p in props)
            {
                sb.AppendFormat("\"{0}\"=\"{1}\",", p.Name, p.GetValue(b));
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    class Base
    {
        public string Name { get; set; }
    }

    class Person : Base
    {
        public string Address { get; set; }
        public int Age { get; set; }
    }

    class Student : Person
    {
        public int Grade { get; set; }
        public int Class { get; set; }
    }
}
