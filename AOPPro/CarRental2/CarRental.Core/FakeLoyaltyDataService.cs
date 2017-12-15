using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    public class FakeLoyaltyDataService : ILoyaltyDataService
    {
        public void AddPoints(Guid customerId, int points)
        {
            Console.WriteLine("客户{0}增加了{1}积分", customerId, points);
        }

        public void SubstractPoints(Guid customerId, int points)
        {
            Console.WriteLine("客户{0}消费了{1}积分", customerId, points);
        }
    }
}
