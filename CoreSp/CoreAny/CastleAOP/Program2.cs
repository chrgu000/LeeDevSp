using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Loader;
using System.IO;

namespace CastleAOP
{
    public class Program2
    {
        // 1.
        public static void Main21(string[] args)
        {
            ProxyGenerator generator = new ProxyGenerator();
            Logger logger = generator.CreateClassProxy<Logger>(new TestInterceptor());

            logger.Write("love");

            Console.Read();
        }

        // 2.接口隔离
        public static void Main22(string[] args)
        {
            ProxyGenerator generator = new ProxyGenerator();
            ILogger logger = generator.CreateClassProxy<Logger1>(new TestInterceptor());

            logger.Write("love");

            Console.Read();
        }

        // 3.重构切面方法
        public static void Main23(string[] args)
        {
            ProxyGenerator generator = new ProxyGenerator();
            ILogger logger = generator.CreateClassProxy<Logger1>(new TestInterceptor2());

            logger.Write("love");

            Console.Read();
        }

        // 4.StandardInterceptor拦截
        public static void Main24(string[] args)
        {
            ProxyGenerator generator = new ProxyGenerator();
            ILogger logger = generator.CreateClassProxy<Logger1>(new TestInterceptor3());

            logger.Write("love");

            Console.Read();
        }

        // 5.StandardInterceptor 多拦截
        public static void Main25(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ProxyGenerator generator = new ProxyGenerator();
            ILogger logger = generator.CreateClassProxy<Logger1>(new TestInterceptor4(), new TestInterceptor3());

            logger.Write("love");

            Console.Read();
        }

        // 6.代理类型 ProxyTypes
        public static void Main26(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ProxyGenerator generator = new ProxyGenerator();
            IStorageNode node = generator.CreateInterfaceProxyWithTargetInterface<IStorageNode>(new StorageNode("master"), new DualNodeInterceptor(new StorageNode("slave")), new CallingLogInterceptor());
            node.Save($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:message"); //应该调用master对象
            node.IsDead = true;
            node.Save($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:message"); //应该调用master对象
            node.Save($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:message"); //应该调用master对象

            Console.Read();
        }

        // 7.运行期使用“合并”方式修改对象行为 Mixin
        public static void Main27(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ProxyGenerator generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(new ClassA());
            ClassB objB = generator.CreateClassProxy<ClassB>(options, new CallingLogInterceptor());
            objB.ActionB();
            InterfaceA objA = objB as InterfaceA;
            objA.ActionA();

            Console.Read();
        }

        // 8.导出、生成代理类型
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string path = Directory.GetCurrentDirectory();
            ModuleScope scope = new ModuleScope(true, false, "Invocation", Path.Combine(path, "invocation.dll"), "Proxy", Path.Combine(path, "Proxy.dll"));
            DefaultProxyBuilder builder = new DefaultProxyBuilder(scope);

            ProxyGenerator generator = new ProxyGenerator(builder);
            ILogger logger = generator.CreateClassProxy<Logger1>(new TestInterceptor2());

            logger.Write("love");

            //scope.SaveAssembly(true); //加这句话可以将动态生成的Invocation类保存到本地硬盘
            //scope.SaveAssembly(false); //加这句话可以将动态生成的Proxy类保存到本地硬盘

            Console.Read();
        }
    }

    #region Simple
    // 1.Simple
    class TestInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.Write("I ");
            invocation.Proceed();
            Console.Write(" you!");
        }
    }

    public class Logger
    {
        public virtual void Write(string msg)
        {
            Console.Write(msg);
        }
    }

    // 2.Simple 接口分离
    public interface ILogger
    {
        void Write(string msg);
    }

    public class Logger1 : ILogger
    {
        public virtual void Write(string msg)
        {
            Console.Write(msg);
        }
    }

    // 3.重构切面方法
    class TestInterceptor2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            PreProcess(invocation);
            invocation.Proceed();
            PostProcess(invocation);
        }

        private void PreProcess(IInvocation invocation)
        {
            Console.Write("I ");
        }

        private void PostProcess(IInvocation invocation)
        {
            Console.Write(" you!");
        }
    }

    // 4.StandardInterceptor拦截
    class TestInterceptor3 : StandardInterceptor
    {
        protected override void PreProceed(IInvocation invocation)
        {
            Console.Write("I ");
        }

        protected override void PostProceed(IInvocation invocation)
        {
            Console.Write(" you!");
        }
    }

    // 5.StandardInterceptor 多拦截
    class TestInterceptor4 : StandardInterceptor
    {
        protected override void PreProceed(IInvocation invocation)
        {
            Console.Write("张三 ");
        }

        protected override void PostProceed(IInvocation invocation)
        {
            Console.Write(" 李四！");
        }
    }
    #endregion

    #region Proxy Types
    // 6.代理类型 ProxyTypes
    public interface IStorageNode
    {
        bool IsDead { get; set; }
        void Save(string message);
    }

    public class StorageNode : IStorageNode
    {
        private string _name;
        public bool IsDead
        {
            get; set;
        }

        public StorageNode(string name)
        {
            _name = name;
        }

        public void Save(string message)
        {
            Console.WriteLine("\"{0}\" was saved to {1}", message, _name);
        }
    }

    public class DualNodeInterceptor : IInterceptor
    {
        private IStorageNode _slave;

        public DualNodeInterceptor(IStorageNode slave)
        {
            _slave = slave;
        }

        public void Intercept(IInvocation invocation)
        {
            IStorageNode master = invocation.InvocationTarget as IStorageNode;
            if (master.IsDead)
            {
                IChangeProxyTarget cpt = invocation as IChangeProxyTarget;
                // 将补代理对象master更换为slave
                cpt.ChangeProxyTarget(_slave);
                // 测试中恢复master状态，以便看到随后调用仍然使用master这一效果
                master.IsDead = false;
            }
            invocation.Proceed();
        }
    }

    public class CallingLogInterceptor : StandardInterceptor
    {
        private int _indent = 0;
        protected override void PreProceed(IInvocation invocation)
        {
            if (_indent > 0)
            {
                Console.Write(" ".PadRight(_indent * 4, ' '));
            }
            _indent++;
            Console.Write("Intercepting:{0}(", invocation.Method.Name);
            if (invocation.Arguments != null && invocation.Arguments.Length > 0)
                for (int i = 0; i < invocation.Arguments.Length; ++i)
                {
                    if (i != 0) Console.Write(",");
                    Console.Write(invocation.Arguments[i] == null ? "null" : invocation.Arguments[i].GetType() == typeof(string) ? "\"" + invocation.Arguments[i].ToString() + "\"":invocation.Arguments[i].ToString());
                }
            Console.WriteLine(")");
        }

        protected override void PostProceed(IInvocation invocation)
        {
            _indent--;
        }
    }
    #endregion

    #region Mixins
    // 7.运行期使用“合并”方式修改对象行为 Mixin
    public interface InterfaceA
    {
        void ActionA();
    }

    public class ClassA : InterfaceA
    {
        public void ActionA()
        {
            Console.WriteLine("I'm from ClassA");
        }
    }

    public class ClassB
    {
        public virtual void ActionB()
        {
            Console.WriteLine("I'm from ClassB");
        }
    }
    #endregion

    #region 导出、生成代理类型
    // 8.导出、生成代理类型

    #endregion
}
