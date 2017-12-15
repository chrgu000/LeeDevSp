using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppEFCore.Model
{
    public class DbQueryCommit : IDisposable
    {
        private readonly EFCoreContext context;
        public DbQueryCommit(EFCoreContext context)
        {
            this.context = context;
        }

        public TEntity Query<TEntity>(params object[] keys) where TEntity : class
        {
            return context.Set<TEntity>().Find(keys);
        }

        public int Commit(Action change)
        {
            change();
            return context.SaveChanges();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return context.Set<TEntity>();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
