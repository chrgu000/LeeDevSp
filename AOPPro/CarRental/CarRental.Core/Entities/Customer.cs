using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DriversLicense { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
