using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleAOP
{
    public class Program
    {
        /// <summary>
        /// 简单的动态代码，拦截virtual方法
        /// </summary>
        /// <param name="args"></param>
        public static void Main01(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ProxyGenerator generator = new ProxyGenerator(); // 实例化 代理类生成器
            SimpleInterceptor interceptor = new SimpleInterceptor(); // 实例化 拦截器

            // 使用 代理类生成器 创建Person对象，而不是使用new关键字来实例化
            Person person = generator.CreateClassProxy<Person>(interceptor);
                        
            //((System.RuntimeType)(person.GetType())).BaseType
            //Console.WriteLine("当前类型：{0}，父类型：{1}", person.GetType(), ((RuntimeType)person.GetType()).BaseType);
            Console.WriteLine();

            person.SayHello();
            Console.WriteLine();

            person.SayName("广东");
            Console.WriteLine();

            person.SayOther(); // 它不是虚方法，无法拦截。


            Console.Read();
        }
    }

    public class Person
    {
        public virtual void SayHello()
        {
            Console.WriteLine("您好！");
        }

        public virtual void SayName(string pHometown)
        {
            Console.WriteLine("同是天涯人，本人自{0}", pHometown);
        }

        public void SayOther()
        {
            Console.WriteLine("是的，咱中国人。");
        }
    }

    public class SimpleInterceptor : StandardInterceptor
    {
        protected override void PreProceed(IInvocation invocation)
        {
            Console.WriteLine("调用前的拦截器，方法名是：{0}", invocation.Method.Name);
            base.PreProceed(invocation);
        }

        protected override void PerformProceed(IInvocation invocation)
        {
            Console.WriteLine("拦截的方法返回时调用的拦截器，方法名是：{0}", invocation.Method.Name);
            base.PerformProceed(invocation);
        }

        protected override void PostProceed(IInvocation invocation)
        {
            Console.WriteLine("调用后的拦截器，方法名是：{0}", invocation.Method.Name);
            base.PostProceed(invocation);
        }
    }
}
