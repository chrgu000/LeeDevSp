using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core.Entities
{
    /// <summary>
    /// 发票
    /// </summary>
    public class Invoice
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
        public int CostPerDay { get; set; }
        public decimal Discount { get; set; }
    }
}
