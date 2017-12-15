using ConsoleAppDapper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace ConsoleAppDapper.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(IDbConnection conn) : base(conn)
        {
        }

        public bool DeleteBoth(int id)
        {
            using (_conn)
            {
                IDbTransaction transaction = _conn.BeginTransaction();
                try
                {
                    string query = "DELETE FROM Book WHERE Id=@Id";
                    string query2 = "DELETE FROM BookReview WHERE BookId=@BookId";
                    _conn.Execute(query2, new { BookId = id }, transaction);
                    _conn.Execute(query, new { Id = id }, transaction);
                    // 提交事务
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            return true;
        }

        public Book QueryBook(int id)
        {
            string query = "SELECT * FROM Book b LEFT JOIN BookReview br ON br.BookId=b.Id WHERE b.Id=@Id";
            Book lookup = null;
            var b = _conn.Query<Book, BookReview, Book>(query, (book, bookReview) =>
            {
                // 扫描第一条记录，判断非空和非重复
                if (lookup == null || lookup.Id != book.Id)
                    lookup = book;
                // 书对应的书评，加入当前书的书评list中，最后把重复的书去掉
                if (bookReview != null)
                    lookup.Reviews.Add(bookReview);
                return lookup;
            }, new { Id = id }).Distinct().SingleOrDefault();
            return b;
        }
    }
}
