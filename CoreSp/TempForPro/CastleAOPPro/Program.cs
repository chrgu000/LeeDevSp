using Castle.DynamicProxy;
using System;

namespace CastleAOPPro
{
    class Program
    {
        static void Main1(string[] args)
        {
            Test();

            Console.Read();
        }

        static void Test()
        {
            ProxyGenerator proxyGenerator = new ProxyGenerator();
            IMyCompare oneCompare = proxyGenerator.CreateInterfaceProxyWithTarget<IMyCompare>(new MyCompareOne(), new MyStandradInterceptor());
            IMyCompare twoCompare = proxyGenerator.CreateInterfaceProxyWithTarget<IMyCompare>(new MyCompareTwo(), new MyStandradInterceptor());

            Type oneCompareType = oneCompare.GetType();
            Type twoCompareType = twoCompare.GetType();

            Console.WriteLine("{0}", ReferenceEquals(oneCompareType, twoCompareType)); // false

            IMyCompare oneCompare1 = proxyGenerator.CreateInterfaceProxyWithTargetInterface<IMyCompare>(new MyCompareOne(), new MyStandradInterceptor());
            IMyCompare twoCompare1 = proxyGenerator.CreateInterfaceProxyWithTargetInterface<IMyCompare>(new MyCompareTwo(), new MyStandradInterceptor());
            Type oneCompare1Type = oneCompare1.GetType();
            Type twoCompare1Type = twoCompare1.GetType();
            Console.WriteLine("{0}", ReferenceEquals(oneCompare1Type, twoCompare1Type)); // ture
        }
    }

    public interface IMyCompare
    {

    }

    public class MyCompareOne : IMyCompare
    {

    }

    public class MyCompareTwo : IMyCompare
    {

    }

    public class MyStandradInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //invocation.Method
        }
    }
}
