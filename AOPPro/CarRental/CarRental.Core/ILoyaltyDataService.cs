using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    public interface ILoyaltyDataService
    {
        void AddPoints(Guid customerId, int points);
        void SubstractPoints(Guid customerId, int points);
    }
}
