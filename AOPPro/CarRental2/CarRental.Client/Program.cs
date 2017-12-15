using CarRental.Core;
using CarRental.Core.Entities;
using System;
using System.Text;

namespace CarRental.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SimulateAddingPoints(); // 模拟累积
            "******************".Output();
            SimulateSubstractPoints(); // 模拟兑换

            Console.Read();
        }

        private static void SimulateAddingPoints()
        {
            var dataService = new FakeLoyaltyDataService(); // 模拟数据库服务
            var service = new LoyaltyAccrualService(dataService);
            var agreement = new RentalAgreement
            {
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "Jackie",
                    DateOfBirth = new DateTime(1999, 1, 1),
                    DriversLicense = "12345678"
                },
                Vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "Ford",
                    Model = "小奔",
                    Size = Size.Compact,
                    Vin = "粤BABC12"
                },
                StartDate = DateTime.Now.AddDays(-3),
                EndDate = DateTime.Now,
                Id = Guid.NewGuid()
            };
            service.Accrue(agreement);
        }

        private static void SimulateSubstractPoints()
        {
            var dataService = new FakeLoyaltyDataService(); // 模拟数据库服务
            var service = new LoyaltyRedemptionService(dataService);
            var invoice = new Invoice
            {
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "Katty",
                    DateOfBirth = new DateTime(1998, 1, 1),
                    DriversLicense = "654321"
                },
                Vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "奥迪",
                    Model = "Q7",
                    Size = Size.SUV,
                    Vin = "粤B89898"
                },
                CostPerDay = 100,
                Id = Guid.NewGuid()
            };
            service.Rdeem(invoice, 3); // 兑换3天
        }
    }

    static class Extensions
    {
        public static void Output(this object obj)
        {
            Console.WriteLine(obj);
        }
    }
}