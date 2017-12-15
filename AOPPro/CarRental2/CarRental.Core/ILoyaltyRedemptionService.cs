using CarRental.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    /// <summary>
    /// 兑换积分
    /// </summary>
    public interface ILoyaltyRedemptionService
    {
        void Rdeem(Invoice invoice, int numberOfDays);
    }
}
