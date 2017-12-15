using System;
using System.Collections.Generic;
using System.Text;
using CarRental.Core.Entities;

namespace CarRental.Core
{
    public class LoyaltyRedemptionService : ILoyaltyRedemptionService
    {
        private readonly ILoyaltyDataService _loyaltyDataService;
        private readonly IExceptionHandler _exceptionHandler; // 异常处理接口
        private readonly ITransactionManager _transactionManager; // 事务管理者

        public LoyaltyRedemptionService(ILoyaltyDataService loyaltyDataService, IExceptionHandler exceptionHandler, ITransactionManager transactionManager)
        {
            _loyaltyDataService = loyaltyDataService;
            _exceptionHandler = exceptionHandler;
            _transactionManager = transactionManager;
        }

        public void Rdeem(Invoice invoice, int numberOfDays)
        {
            // 防御性编程
            if (invoice == null)
            {
                throw new Exception("invoice为null！");
            }
            if (numberOfDays <= 0)
            {
                throw new Exception("numberOfDays不能为小于1！");
            }
            // logging
            Console.WriteLine("Redem:{0}", DateTime.Now);
            Console.WriteLine("Invoice:{0}", invoice.Id);

            _exceptionHandler.Wrapper(() =>
            {
                _transactionManager.Wrapper(() =>
                {
                    var pointsPerDay = 10;
                    if (invoice.Vehicle.Size >= Size.Luxury)
                    {
                        pointsPerDay = 15;
                    }
                    var totalPoints = pointsPerDay * numberOfDays;
                    invoice.Discount = numberOfDays * invoice.CostPerDay;
                    _loyaltyDataService.SubstractPoints(invoice.Customer.Id, totalPoints);

                    // logging
                    Console.WriteLine("Redem completed:{0}", DateTime.Now);
                });
            });
        }
    }
}
