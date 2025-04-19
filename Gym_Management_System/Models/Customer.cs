using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models
{
    public class Customer : User
    {
        [Required]
        public MembershipType MembershipType { get; set; }

        [Required]
        public MembershipStatus MembershipStatus { get; set; }

        [Required]
        public DateTime SubscriptionDate { get; set; }

        public DateTime? MembershipExpiry { get; set; }

        [Required]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [Required]
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public int GymBranchId { get; set; }
        public GymBranch? GymBranch { get; set; }

        public Customer()
        {
            SubscriptionDate = DateTime.UtcNow;
        }
    }

    public enum MembershipType
    {
        Monthly,
        Quarterly,
        Yearly
    }

    public enum MembershipStatus
    {
        Active,
        Expired,
        Suspended
    }
}
