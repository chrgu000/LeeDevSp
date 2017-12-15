using CarRental.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    public interface ILoyaltyAccrualService
    {
        /// <summary>
        /// 累计积分
        /// </summary>
        /// <param name="agreement"></param>
        void Accrue(RentalAgreement agreement);
    }
}
