// Payment.cs
namespace GymManagement.Models;
public class Payment
{
  public required int PaymentId { get; set; }
  public required decimal Price { get; set; }
  public required int CustomerId { get; set; } // fk
  public required Customer Customer { get; set; }
  public required string PaymentMethod { get; set; }  // e.g., Cash, Card
  public required DateTime PaymentDate { get; set; }
}
