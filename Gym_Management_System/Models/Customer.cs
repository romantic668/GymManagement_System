// Customer.cs
namespace GymManagement.Models;
public class Customer : User
{
  public required string MembershipType { get; set; }
  public required DateTime SubscriptionDate { get; set; }
  public required ICollection<Booking> Bookings { get; set; } = new List<Booking>();

  public required ICollection<Payment> Payments { get; set; } = new List<Payment>();

  //  Add a default constructor (EF Core needs it)
  public Customer()
  {
    SubscriptionDate = DateTime.UtcNow;  // Default to current date
  }
}
