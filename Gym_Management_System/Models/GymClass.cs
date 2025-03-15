// GymClass.cs
namespace GymManagement.Models;
public class GymClass
{
  public required int GymClassId { get; set; }
  public required string ClassName { get; set; }
  public required DateTime AvailableTime { get; set; }
  public required int Duration { get; set; } // In minutes
  public string? Description { get; set; }
  public required int TrainerId { get; set; } = 1; // fk
  public Trainer? Trainer { get; set; }
  public required ICollection<Session> Sessions { get; set; }
}
