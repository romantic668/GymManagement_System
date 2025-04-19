using GymManagement.Models;


namespace GymManagement.Helpers
{

    public static class PricingPlans
    {
        public static readonly Dictionary<MembershipType, decimal> Prices = new()
        {
            { MembershipType.Monthly, 59.99m },
            { MembershipType.Quarterly, 149.99m },
            { MembershipType.Yearly, 499.99m }
        };

        public static decimal GetPrice(MembershipType type) => Prices[type];

        public static string GetDisplayName(MembershipType type)
        {
            return type switch
            {
                MembershipType.Monthly => "Monthly (+1 month)",
                MembershipType.Quarterly => "Quarterly (+3 months)",
                MembershipType.Yearly => "Yearly (+12 months)",
                _ => "Unknown"
            };
        }
    }
}
