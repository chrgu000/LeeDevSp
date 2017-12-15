using System;
using System.Collections.Generic;
using System.Text;
using CarRental.Core.Entities;

namespace CarRental.Core
{
    public class LoyaltyRedemptionService : ILoyaltyRedemptionService
    {
        private readonly ILoyaltyDataService _loyaltyDataService;

        public LoyaltyRedemptionService(ILoyaltyDataService loyaltyDataService)
        {
            _loyaltyDataService = loyaltyDataService;
        }

        public void Rdeem(Invoice invoice, int numberOfDays)
        {
            var pointsPerDay = 10;
            if(invoice.Vehicle.Size >= Size.Luxury)
            {
                pointsPerDay = 15;
            }
            var totalPoints = pointsPerDay * numberOfDays;
            invoice.Discount = numberOfDays * invoice.CostPerDay;
            _loyaltyDataService.SubstractPoints(invoice.Customer.Id, totalPoints);
        }
    }
}
