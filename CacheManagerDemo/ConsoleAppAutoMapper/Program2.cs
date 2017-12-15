using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppAutoMapper
{
    class Program2
    {
        static void Main(string[] args)
        {
            var customer = new Customer { Name = "George Costanza" };
            var order = new Order { Customer = customer };
            var bosco = new Product { Name = "Bosco", Price = 9.25M };
            order.AddOrderLineItem(bosco, 15);

            // 配置AutoMapper
            AutoMapper.Mapper.Initialize(cfg=>cfg.CreateMap<)
               
            Console.Read();
        }
    }

    class Order
    {

        private readonly IList<OrderLineItem> _orderLineItems = new List<OrderLineItem>();
        public Customer Customer { get; set; }
        public OrderLineItem[] GetOrderLineItems()
        {
            return _orderLineItems.ToArray();
        }

        public void AddOrderLineItem(Product product, int quantity)
        {
            _orderLineItems.Add(new OrderLineItem(product, quantity));
        }

        public decimal GetTotal()
        {
            return _orderLineItems.Sum(oli => oli.GetTotal());
        }
    }

    public class Product
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
    }

    class OrderLineItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public OrderLineItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        internal decimal GetTotal()
        {
            return Quantity * Product.Price;
        }
    }

    class OrderDto
    {
        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }

    public static class AutoMapperExtensions
    {
        public static void CreateMap<F,T>()
    }
}
