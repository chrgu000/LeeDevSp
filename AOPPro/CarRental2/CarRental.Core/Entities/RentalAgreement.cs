using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core.Entities
{
    /// <summary>
    /// 租车用户条款
    /// </summary>
    public class RentalAgreement
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
