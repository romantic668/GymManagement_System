// GymBranch.cs
namespace GymManagement.Models;
public class GymBranch
{
  public required int BranchId { get; set; }
  public required string BranchName { get; set; }
  public string? Address { get; set; }
  public string? ContactNumber { get; set; }
  public required ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();  // 1..*
  public required ICollection<Receptionist> Receptionists { get; set; } = new List<Receptionist>(); // 1..*
  public required ICollection<Room> Rooms { get; set; } = new List<Room>(); // 1..*
}
