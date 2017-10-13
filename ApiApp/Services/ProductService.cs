using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiApp.Models;

namespace ApiApp.Services
{
    public class ProductService : IProductService
    {
        private List<Product> _products;

        public ProductService()
        {
            _products = new List<Product>
            {
                new Product{ Id = 1, Name = "小米2", Price = 2299},
                new Product{ Id = 2, Name = "小米3", Price = 2399},
                new Product{ Id = 3, Name = "小米4", Price = 2499}
            };
        }

        public int NewId => _products.Count == 0 ? 1 : _products.Max(p => p.Id) + 1;

        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void Delete(int id)
        {
            var product = Get(id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

        public Product Get(int id)
        {
            return _products.SingleOrDefault(p => p.Id == id);
        }

        public List<Product> GetList()
        {
            return _products;
        }

        public void Update(Product product)
        {
            if (product == null)
                return;
            var pr = _products.Find(p => p.Id == product.Id);
            if (pr != null)
            {
                _products.Remove(pr);
                _products.Add(product);
            }
        }
    }
}
