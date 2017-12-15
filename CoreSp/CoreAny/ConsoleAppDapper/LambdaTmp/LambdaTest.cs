using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleAppDapper.LambdaTmp
{
    public class LambdaTest
    {
        public void CallMethod()
        {
            var helloWorld = Activator.CreateInstance<HelloWorld>();

            var method = helloWorld.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("SayHello"));
            if (method == null)
            {
                Console.WriteLine("there is not any method name called 'SayHello'");
                return;
            }
            ConstantExpression x = Expression.Constant("Hello World", typeof(string));
            ConstantExpression y = Expression.Constant("How are you?");

            var instance = Expression.Convert(Expression.Constant(helloWorld), typeof(HelloWorld));
            MethodCallExpression sayHello = Expression.Call(instance, method, x, y); // helloWorld.SayHello(x,y);
            var sayHelloLambda = Expression.Lambda<Action>(sayHello);
            sayHelloLambda.Compile()();
        }

        public void CreateMethod()
        {
            Expression<Func<int, int, int>> calc = (x, y) => x + y;
            //var a = Expression.Variable(typeof(int), "a");
            //var b = Expression.Variable(typeof(int), "b");
            int a = 10, b = 12;
            var res = calc.Compile()(a, b);
            Console.WriteLine(res);

            var num1 = Expression.Parameter(typeof(double), "num1");
            var num2 = Expression.Parameter(typeof(double), "num2");
            var multiply = Expression.Multiply(num1, num2);
            var resExp = Expression.Lambda<Func<double, double, double>>(multiply, num1, num2);
            var res1 = resExp.Compile()(12.5, 19.9);
            Console.WriteLine(res1);
        }

        public void MemberCopy()
        {
            Person p = new Person { Name = "Chinese", Age = 28 };
            p.Show();
            var p1 = Operator<Person>.ShallowCopy(p);
            p1.Show();

        }

        public void PropertyEquals()
        {
            Person p1 = new Person { Name = "Chinese", Age = 28 };
            Person p2 = new Person { Name = "Chinese", Age = 28 };
            if (Operator<Person>.ObjectPropertyEqual(p1, p2))
            {
                Console.WriteLine("两个对象所有属性值都相等！");
            }
            else
            {
                Console.WriteLine("两个对象所有属性值并不都相等！");
            }
        }

        public void LoopPrint()
        {
            var exitFor = Expression.Label("exitFor"); // jump label
            var x = Expression.Variable(typeof(int), "x");
            var body = Expression.Block(
                new[] { x }, // declare scope variables
                Expression.Assign(x, Expression.Constant(0, typeof(int))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThanOrEqual( // test for exit                            
                            x, Expression.Constant(10, typeof(int))
                    ),
                    Expression.Break(exitFor), // perform exit
                    Expression.Block(// perform code
                        Expression.Call(typeof(Console), "WriteLine", null, x),
                        Expression.PostIncrementAssign(x)
                        )
                   ),
                    exitFor
                    ) // Loop ends
                );
            var runtimeLoop = Expression.Lambda<Action>(body).Compile();
            runtimeLoop();
        }

        public void ExpSqlIn()
        {
            var persons = new List<Person>
            {
                new Person { Name="person1", Age=20 },
                new Person { Name="person2", Age=21 },
                new Person { Name="person3", Age=22 },
                new Person { Name="person4", Age=23 },
                new Person { Name="person5", Age=24 },
                new Person { Name="person6", Age=25 },
                new Person { Name="person7", Age=26 },
                new Person { Name="person8", Age=27 }
            };

            var res = persons.AsQueryable().WhereIn(p => p.Age, new int[] { 21,22,25,26,23 });
            res.ToList().ForEach(p =>
            {
                p.Show();
            });
        }
    }

    public class HelloWorld
    {
        public void SayHello(string str1, string str2)
        {
            var msg = $"From SayHello:{str1},{str2}";
            Console.WriteLine(msg);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{Name},{Age}";
        }

        public void Show()
        {
            Console.WriteLine(this);
        }
    }

    public static class Operator<T>
    {
        private static readonly Func<T, T> ShallowClone;
        private static readonly Func<T, T, bool> PropsEqual;

        //------------------------------ copy ---
        public static T ShallowCopy(T sourceObj)
        {
            return ShallowClone.Invoke(sourceObj);
        }

        public static bool ObjectPropertyEqual(T obj1, T obj2)
        {
            return PropsEqual(obj1, obj2);
        }

        static Operator()
        {
            //------------------------------ copy ---
            var origParam = Expression.Parameter(typeof(T), "orig");
            var setProps = from prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                           where prop.CanRead && prop.CanWrite
                           select (MemberBinding)Expression.Bind(prop, Expression.Property(origParam, prop));
            var copy = Expression.MemberInit(Expression.New(typeof(T)), setProps);
            ShallowClone = Expression.Lambda<Func<T, T>>(copy, origParam).Compile();

            //------------------------------ equals ---
            var x = Expression.Parameter(typeof(T), "x");
            var y = Expression.Parameter(typeof(T), "y");
            // 获取类型T上可读Property
            var readableProps = from prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                where prop.CanRead
                                select prop;
            Expression combination = null;
            foreach (var readableProp in readableProps)
            {
                var thisPropEqual = Expression.Equal(Expression.Property(x, readableProp), Expression.Property(y, readableProp));
                if (combination == null)
                {
                    combination = thisPropEqual;
                }
                else
                {
                    combination = Expression.AndAlso(combination, thisPropEqual);
                }
            }
            if (combination == null) // 如果没有需要比较的东西直接返回false
            {
                PropsEqual = (p1, p2) => false;
            }
            else
            {
                PropsEqual = Expression.Lambda<Func<T, T, bool>>(combination, x, y).Compile();
            }
        }

        //------------------------------ equals ---
    }


    public static class QueryableExtensions
    {
        /// <summary>
        /// 使之支持Sql in语法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="query"></param>
        /// <param name="obj"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIn<T, TValue>(this IQueryable<T> query, Expression<Func<T, TValue>> obj, IEnumerable<TValue> values)
        {
            return query.Where(BuildContainsExpression(obj, values));
        }

        private static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector)
            {
                throw new ArgumentNullException("valueSelector");
            }
            if (null == values)
            {
                throw new ArgumentNullException("values");
            }
            var p = valueSelector.Parameters.Single();
            if (!values.Any()) return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate(Expression.Or);
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}
