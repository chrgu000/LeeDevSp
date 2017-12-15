using ConsoleAppDapper.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Dapper;
using ConsoleAppDapper.Entities;
using System.Linq.Expressions;

namespace ConsoleAppDapper.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected IDbConnection _conn;

        public BaseRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        protected string GetTableName()
        {
            return typeof(T).Name;
        }

        protected IList<string> Fields
        {
            get
            {
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                return properties.Select(p => p.Name).OrderBy(o => o).ToList();
            }
        }

        public bool Add(T item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0} (", GetTableName());
            StringBuilder sbV = new StringBuilder();
            sbV.Append(" VALUES (");
            foreach (var p in Fields)
            {
                if (p.Equals("Id"))
                    continue;
                sb.AppendFormat("{0},", p);
                sbV.AppendFormat("@{0},", p);
            }
            sb.Remove(sb.Length - 1, 1);
            sbV.Remove(sbV.Length - 1, 1);
            sbV.Append(")");
            sb.Append(")").Append(sbV.ToString());

            return _conn.Execute(sb.ToString(), item) > 0;
        }

        public bool Delete(T item)
        {
            string query = $"DELETE FROM {GetTableName()} WHERE Id=@Id";
            return _conn.Execute(query, item) == 1;
        }

        public bool Update(T item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", GetTableName());
            foreach (var p in Fields)
            {
                if (p.Equals("Id")) continue;
                sb.AppendFormat("{0}=@{0}", p);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.AppendFormat(" WHERE Id={0}", item.Id);
            return _conn.Execute(sb.ToString(), item) > 0;
        }

        public T Query(int id)
        {
            string fields = string.Join(",", Fields.ToArray());
            string query = $"SELECT {fields} FROM {GetTableName()} WHERE Id={id}";
            return _conn.Query<T>(query, new { Id = id }).SingleOrDefault();
        }

        public IEnumerable<T> Query(Expression<Predicate<T>> condition)
        {
            string fields = string.Join(",", Fields.ToArray());
            string query = $"SELECT {fields} FROM {GetTableName()}";
            return _conn.Query<T>(query);
        }
    }
}
