using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace CastleAOPPro
{
    class Program2
    {
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            new IocConfig().Load(containerBuilder);

            var container = containerBuilder.Build(Autofac.Builder.ContainerBuildOptions.None);

            DbContextManager.Instance.Initialize(container.Resolve<CADbContext>()); // 由其代管

            var service = container.Resolve<IService>();
            service.Add("How to use the EF.Core", "The introduction of the EF.Core for the new users who nerver use the .net core & .net core ef before.");

            service.Add("How to use the EF.Core2", "The introduction of the EF.Core for the new users who nerver use the .net core & .net core ef before.");

            var article1 = service.Get(1);
            var article2 = service.Get(2);
            if (article1 != null && article2 != null)
            {
                //article2.Code = article1.Code;
                //service.Update(article2);

                service.BatchBusi(() =>
                {
                    article2.Code = article1.Code;
                    service.Add("What is the name of your sister?", "Just for fun");
                    service.Update(article2); // 此处由于重复键抛出异常，导致事务回滚
                });

            }

            Console.Read();
        }


    }

    /// <summary>
    /// 开启事务属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TransactionCallHandlerAttribute : Attribute
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 事务范围
        /// </summary>
        public TransactionScopeOption ScopeOption { get; set; }

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        public TransactionCallHandlerAttribute()
        {
            Timeout = 60;
            ScopeOption = TransactionScopeOption.Required;
            IsolationLevel = IsolationLevel.ReadCommitted;
        }
    }

    public interface ILog
    {
        void Log(string msg);
    }

    public class Logger : ILog
    {
        public void Log(string msg)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {msg}");
        }
    }

    public class LogManager
    {
        private static LogManager _instance = new LogManager();
        private LogManager() { }

        public static LogManager Instance
        {
            get { return _instance; }
        }

        public static ILog GetLog()
        {
            return new Logger();
        }
    }

    /// <summary>
    /// 事务 拦截器（适用于老式windows事务）
    /// </summary>
    /*public class TransactionInterceptor : IInterceptor
    {
        private static readonly ILog _logger = LogManager.GetLog();

        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }
            TransactionCallHandlerAttribute transactionAttr = methodInfo.GetCustomAttributes<TransactionCallHandlerAttribute>(true).FirstOrDefault();
            if (transactionAttr != null)
            {
                TransactionOptions transactionOptions = new TransactionOptions();
                // 设置事务隔离级别
                transactionOptions.IsolationLevel = transactionAttr.IsolationLevel;
                // 设置事务超时 秒
                transactionOptions.Timeout = new TimeSpan(0, 0, transactionAttr.Timeout);
                using (TransactionScope scop = new TransactionScope(transactionAttr.ScopeOption, transactionOptions))
                {
                    try
                    {
                        _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} begin processing in transaction...");
                        // 实现事务性工作
                        invocation.Proceed();
                        scop.Complete();
                        _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} processed completely in transaction.");
                    }
                    catch (Exception ex)
                    {
                        // 记录异常
                        _logger.Log(ex.Message);
                        throw ex;
                    }
                }
            }
            else
            {
                try
                {
                    _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} begin processing...");
                    // 没有事务时直接执行方法
                    invocation.Proceed();
                    _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} processed completely.");
                }
                catch (Exception ex)
                {
                    _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name}:\n{ex.Message}");
                }
            }
        }
    }*/

    ///<summary>
    ///事务 拦截器
    ///</summary>
    public class TransactionInterceptor : IInterceptor
    {
        private static readonly ILog _logger = LogManager.GetLog();
        private static DbContext _dbContext = DbContextManager.Instance.Context;

        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }
            TransactionCallHandlerAttribute transactionAttr = methodInfo.GetCustomAttributes<TransactionCallHandlerAttribute>(true).FirstOrDefault();
            if (transactionAttr != null)
            {
                //TransactionOptions transactionOptions = new TransactionOptions();
                //// 设置事务隔离级别
                //transactionOptions.IsolationLevel = transactionAttr.IsolationLevel;
                //// 设置事务超时 秒
                //transactionOptions.Timeout = new TimeSpan(0, 0, transactionAttr.Timeout);
                using (IDbContextTransaction transaction = _dbContext.Database.BeginTransaction(transactionAttr.IsolationLevel.Map()))
                {
                    try
                    {
                        _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} begin processing in transaction...");
                        // 实现事务性工作
                        invocation.Proceed();
                        transaction.Commit();
                        _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} processed completely in transaction.");
                    }
                    catch (Exception ex)
                    {
                        // 记录异常
                        _logger.Log(ex.Trace());
                        transaction.Rollback();
                        // throw ex;
                    }
                }
            }
            else
            {
                // 当测试事务时，此处不应该有异常捕获
                //try
                //{
                _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} begin processing...");
                // 没有事务时直接执行方法
                invocation.Proceed();
                _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name} processed completely.");
                //}
                //catch (Exception ex)
                //{
                //    _logger.Log($"{methodInfo.DeclaringType.Name}.{methodInfo.Name}:\n{ex.Trace()}");
                //}
            }
        }
    }

    public static class Extensions
    {
        public static System.Data.IsolationLevel Map(this IsolationLevel level)
        {
            System.Data.IsolationLevel isolationLevel;
            switch (level)
            {
                case IsolationLevel.Chaos:
                    isolationLevel = System.Data.IsolationLevel.Chaos; break;
                case IsolationLevel.ReadCommitted:
                    isolationLevel = System.Data.IsolationLevel.ReadCommitted; break;
                case IsolationLevel.ReadUncommitted:
                    isolationLevel = System.Data.IsolationLevel.ReadUncommitted; break;
                case IsolationLevel.RepeatableRead:
                    isolationLevel = System.Data.IsolationLevel.RepeatableRead; break;
                case IsolationLevel.Serializable:
                    isolationLevel = System.Data.IsolationLevel.Serializable; break;
                case IsolationLevel.Snapshot:
                    isolationLevel = System.Data.IsolationLevel.Snapshot; break;
                case IsolationLevel.Unspecified:
                    isolationLevel = System.Data.IsolationLevel.Unspecified; break;
                default:
                    isolationLevel = System.Data.IsolationLevel.Unspecified; break;
            }
            return isolationLevel;
        }

        public static string Trace(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            if (ex != null)
            {
                sb.Append($"{ex.Message}\n");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    sb.Append($"Inner Exception: {ex.Message}\n");
                }
            }
            return sb.ToString();
        }
    }

    public interface IDependency
    {

    }

    public interface IRepository : IDependency
    {
        void Add(Article entity);
        void Update(Article article);

        Article Get(int id);
    }

    public class RepositoryBase : IRepository
    {
        private CADbContext _context;

        public RepositoryBase(CADbContext context)
        {
            _context = context;
        }

        public void Add(Article entity)
        {
            //_context.Set<Article>().Add(entity);
            _context.Add(entity);
            _context.SaveChanges();
        }

        public Article Get(int id)
        {
            return _context.Set<Article>().FirstOrDefault(a => a.Id == id);
        }

        public void Update(Article article)
        {
            _context.Update(article);
            _context.SaveChanges();
        }
    }

    public interface IService : IDependency
    {
        void Add(string name, string content);
        void Update(Article article);
        Article Get(int id);

        void BatchBusi(Action action);
    }

    public class BaseService : IService
    {
        private IRepository _repository;

        public BaseService(IRepository repository)
        {
            _repository = repository;
        }

        public void Add(string name, string content)
        {
            _repository.Add(new Article
            {
                Code = Guid.NewGuid().ToString().Replace("-", ""),
                Name = name,
                Content = content,
                CreatedBy = "Jackie",
                CreatedOn = DateTime.Now
            });
        }

        [TransactionCallHandler]
        public void BatchBusi(Action action)
        {
            action?.Invoke();
        }

        public Article Get(int id)
        {
            return _repository.Get(id);
        }

        //[TransactionCallHandler]
        public void Update(Article article)
        {
            _repository.Update(article);
        }
    }

    public class Article
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Content { get; set; }
    }

    public class CADbContext : DbContext
    {
        private string _connectionString;

        public CADbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>(d =>
            {
                d.ToTable("Article");
                d.HasKey(a => a.Id);
            });
        }
    }

    public class DbContextManager
    {
        private static DbContextManager _instance = new DbContextManager();
        private DbContextManager() { }

        private DbContext _dbContext;

        public static DbContextManager Instance
        {
            get { return _instance; }
        }

        public void Initialize(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbContext Context
        {
            get { return _dbContext; }
        }
    }

    public class IocConfig
    {
        public void Load(ContainerBuilder builder)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            builder.RegisterType<TransactionInterceptor>();
            builder.RegisterInstance(new CADbContext("Data Source=.;Initial Catalog=ProductDB;User Id=sa;Pwd=1234")).SingleInstance(); // dbcontext
            builder.RegisterAssemblyTypes(assembly)
                .Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(TransactionInterceptor));
        }
    }
}
