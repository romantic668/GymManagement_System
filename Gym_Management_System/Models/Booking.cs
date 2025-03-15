// Booking.cs
namespace GymManagement.Models;
public class Booking
{
  public int BookingId { get; set; }
  public required DateTime BookingDate { get; set; }
  public required BookingStatus Status { get; set; }  // Pending, Confirmed, Canceled）
  public int? CustomerId { get; set; } // fk
  public Customer? Customer { get; set; }
  public int? SessionId { get; set; }  // fk
  public Session? Session { get; set; }
  public DateTime? CheckInTime { get; set; }  // Check-in time (if checked-in)

  public int? ReceptionistId { get; set; }  // The receptionist who handles the check-in (optional)
  public Receptionist? Receptionist { get; set; }

}
public enum BookingStatus
{
  Pending,
  Confirmed,
  Canceled,
  CheckedIn
}
