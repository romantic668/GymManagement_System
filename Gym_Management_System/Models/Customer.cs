// Customer.cs
namespace GymManagement.Models;
public class Customer : User
{
  public required string MembershipType { get; set; }
  public required DateTime SubscriptionDate { get; set; }
  public required ICollection<Booking> Bookings { get; set; }
  public required ICollection<Payment> Payments { get; set; }
}
