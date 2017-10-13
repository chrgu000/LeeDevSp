using ApiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApp.Services
{
    public interface IProductService
    {
        void Add(Product product);

        void Update(Product product);

        void Delete(int id);

        Product Get(int id);

        List<Product> GetList();

        int NewId { get; }
    }
}
