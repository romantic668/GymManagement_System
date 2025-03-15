// Session.cs
namespace GymManagement.Models;
public class Session
{
  public required int SessionId { get; set; }
  public required DateTime SessionDateTime { get; set; }
  public required int Capacity { get; set; }
  public required int GymClassId { get; set; } // fk
  public required GymClass GymClass { get; set; }
  public required int RoomId { get; set; } // fk
  public required Room Room { get; set; }
  public required int TrainerId { get; set; } = 1;// fk
  public required Trainer Trainer { get; set; }
  public required ICollection<Booking> Bookings { get; set; }
  public int? ReceptionistId { get; set; }
  public Receptionist? Receptionist { get; set; }
}
