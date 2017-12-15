using System;
using System.Collections.Generic;
using System.Text;
using CarRental.Core.Entities;

namespace CarRental.Core
{
    public class LoyaltyAccrualService : ILoyaltyAccrualService
    {
        private readonly ILoyaltyDataService _loyaltyDataService;

        public LoyaltyAccrualService(ILoyaltyDataService loyaltyDataService)
        {
            _loyaltyDataService = loyaltyDataService; // 数据服务必须在该对象初始时传入该对象
        }

        /// <summary>
        /// 包含积分系统累计客户积分逻辑和规则
        /// </summary>
        /// <param name="agreement"></param>
        public void Accrue(RentalAgreement agreement)
        {
            var rentalTimeSpan = agreement.EndDate.Subtract(agreement.StartDate);
            var numberOfDays = (int)rentalTimeSpan.TotalDays;
            var pointsPerDay = 1;
            if(agreement.Vehicle.Size >= Size.Luxury)
            {
                pointsPerDay = 2;
            }
            var points = pointsPerDay * numberOfDays;
            // 调用数据服务存储客户获得的积分
            _loyaltyDataService.AddPoints(agreement.Customer.Id, points);
        }
    }
}
