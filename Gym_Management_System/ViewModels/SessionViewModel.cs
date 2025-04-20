using GymManagement.Models; 
public class SessionViewModel
{
  public int SessionId { get; set; }
  public string SessionName { get; set; }= "";
  public DateTime SessionDateTime { get; set; }
  public Trainer Trainer { get; set; }= null!;
  public bool IsBookedByCurrentUser { get; set; }

  public required string ClassName { get; set; }= "";
  // public required DateTime SessionDate { get; set; }
  public required string RoomName { get; set; }= "";
  public required int TotalBookings { get; set; }

  public required SessionCategory Category { get; set; }
}

